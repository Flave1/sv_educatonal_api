using BLL.Constants;
using BLL.LoggerService;
using Contracts.Class;
using Contracts.Common;
using DAL;
using DAL.ClassEntities;
using DAL.StudentInformation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SMP.BLL.Constants;
using SMP.BLL.Services.Constants;
using SMP.BLL.Utilities;
using SMP.Contracts.ClassModels;
using SMP.DAL.Models.ClassEntities;
using SMP.DAL.Models.GradeEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BLL.ClassServices
{
    public class ClassGroupService : IClassGroupService
    {
        private readonly DataContext context;
        private readonly IHttpContextAccessor accessor;
        private readonly ILoggerService loggerService;
        private readonly string smsClientId;
        public ClassGroupService(DataContext context, IHttpContextAccessor accessor, ILoggerService loggerService)
        {
            this.context = context;
            this.accessor = accessor;
            this.loggerService = loggerService;
            smsClientId = accessor.HttpContext.User.FindFirst(x => x.Type == "smsClientId")?.Value;
        }

        async Task<APIResponse<CreateClassGroup>> IClassGroupService.CreateClassGroupAsync(CreateClassGroup request)
        {
            var res = new APIResponse<CreateClassGroup>();
            try
            {
                if (context.SessionClassGroup.Where(x=>x.ClientId == smsClientId).AsEnumerable().Any(r =>request.GroupName.ToLower() ==r.GroupName.ToLower() 
                && request.SessionClassId == r.SessionClassId.ToString()))
                {
                    res.Message.FriendlyMessage = "Group Name Already exist";
                    return res;
                }
                var group = new SessionClassGroup();
                group.GroupName = request.GroupName;
                group.SessionClassId = Guid.Parse(request.SessionClassId);
                group.SessionClassSubjectId = Guid.Parse(request.SessionClassSubjectId);
                group.ListOfStudentContactIds = string.Join(',', request.StudentContactIds);
            
                context.SessionClassGroup.Add(group);
                await context.SaveChangesAsync();
                res.Result = request;
            }
            catch (Exception ex)
            {
                loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                res.Message.FriendlyMessage = Messages.FriendlyException;
                res.Message.TechnicalMessage = ex.ToString();
                return res;
            }
           
            res.IsSuccessful = true;
            res.Message.FriendlyMessage = "You have succesfully created a Class Group";
            return res;
        }

        async Task<APIResponse<UpdateClassGroup>> IClassGroupService.UpdateClassGroupAsync(UpdateClassGroup request)
        {
            var res = new APIResponse<UpdateClassGroup>();
            try
            {
                if (context.SessionClassGroup.Where(x=>x.ClientId == smsClientId).AsEnumerable().Any(r => request.GroupName.ToLower() == r.GroupName.ToLower() 
                && r.SessionClassGroupId != Guid.Parse(request.GroupId) && request.SessionClassId == r.SessionClassId.ToString()))
                {
                    res.Message.FriendlyMessage = "Group Name Already exist";
                    return res;
                }

                var grp = context.SessionClassGroup.FirstOrDefault(r => r.SessionClassGroupId == Guid.Parse(request.GroupId) && r.ClientId == smsClientId);
                if (grp == null)
                {
                    res.Message.FriendlyMessage = "Group Group does not exist";
                    return res;
                }
                grp.GroupName = request.GroupName;
                grp.SessionClassId = Guid.Parse(request.SessionClassId);
                grp.SessionClassSubjectId = Guid.Parse(request.SessionClassSubjectId);
                grp.ListOfStudentContactIds = string.Join(',', request.StudentContactIds);
                await context.SaveChangesAsync();
                res.Result = request;
            }
            catch (Exception ex)
            {
                loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                res.IsSuccessful = false;
                res.Message.FriendlyMessage = Messages.FriendlyException;
                res.Message.TechnicalMessage = ex.ToString();
                return res;
            }

            res.IsSuccessful = true;
            res.Message.FriendlyMessage = "You have succesfully updated a Class Group";
            return res;
        }

        async Task<APIResponse<List<GetClassGroupRequest>>> IClassGroupService.GetAllClassGroupsAsync(Guid sessionClassId, Guid sessionClassSubjectId)
        {
            var res = new APIResponse<List<GetClassGroupRequest>>();
            var student = context.StudentContact.Where(x=>x.ClientId == smsClientId).Include(s => s.User).Where(e => e.SessionClassId == sessionClassId && e.EnrollmentStatus == (int)EnrollmentStatus.Enrolled).ToList();
            
            var result = await context.SessionClassGroup.Where(x => x.ClientId == smsClientId && x.GroupName != "all-students")
                .OrderBy(s => s.GroupName)
                .Include(d => d.SessionClass).ThenInclude(s => s.Class)
                .Include(d => d.SessionClassSubject).ThenInclude(s => s.Subject)
                .Where(d => d.Deleted == false   
                && d.SessionClassSubjectId == sessionClassSubjectId).Select(a => 
                new GetClassGroupRequest(a, student.Count())).ToListAsync();
            res.IsSuccessful = true;
            res.Result = result;
            return res;
        }

        async Task<APIResponse<List<SessionClassSubjects>>> IClassGroupService.GetSessionClassSubjectsAsync(Guid sessionClassId)
        {
            var res = new APIResponse<List<SessionClassSubjects>>();
            var teacherId = accessor.HttpContext.User.FindFirst(e => e.Type == "teacherId")?.Value;

            //GET SUPER ADMIN CLASSES
            if (accessor.HttpContext.User.IsInRole(DefaultRoles.AdminRole(smsClientId)) || accessor.HttpContext.User.IsInRole(DefaultRoles.FLAVETECH))
            {
                res.Result = await context.SessionClassSubject.Where(x => x.ClientId == smsClientId)
                .Include(s => s.Subject)
                .Where(d => d.Deleted == false && d.SessionClassId == sessionClassId && d.Subject.Deleted == false && d.Subject.IsActive == true).Select(a =>
                new SessionClassSubjects(a)).ToListAsync();

                res.Message.FriendlyMessage = Messages.GetSuccess;
                res.IsSuccessful = true;
                return res;
            }

            if (accessor.HttpContext.User.IsInRole(DefaultRoles.TeacherRole(smsClientId)))
            {
                var subjectTeacherSubjects = context.SessionClassSubject.Where(x => x.ClientId == smsClientId)
                    .Include(d => d.Subject)
                    .Where(e => e.SubjectTeacherId == Guid.Parse(teacherId) && e.SessionClassId == sessionClassId && e.Subject.Deleted == false && e.Subject.IsActive == true).Select(s => new SessionClassSubjects(s));

                var formTeacherSubjects = context.SessionClassSubject.Where(x => x.ClientId == smsClientId)
                    .Include(d => d.Subject)
                    .Include(d => d.SessionClass)
                    .Where(e => e.SessionClassId == sessionClassId && e.SessionClass.FormTeacherId == Guid.Parse(teacherId) && e.Subject.Deleted == false && e.Subject.IsActive == true).Select(s => new SessionClassSubjects(s));

                var result = subjectTeacherSubjects.AsEnumerable().Concat(formTeacherSubjects.AsEnumerable());
                res.Result = result.GroupBy(x => new { x.SessionClassSubjectId, x.Subjectid }).Select(s => s.FirstOrDefault()).ToList();

                res.Message.FriendlyMessage = Messages.GetSuccess;
                res.IsSuccessful = true;
                return res;
            }

            if (accessor.HttpContext.User.IsInRole(DefaultRoles.ParentRole(smsClientId)))
            {
                res.Result = await context.SessionClassSubject.Where(x => x.ClientId == smsClientId)
                .Include(s => s.Subject)
                .Where(d => d.Deleted == false && d.SessionClassId == sessionClassId && d.Subject.Deleted == false && d.Subject.IsActive == true).Select(a =>
                new SessionClassSubjects(a)).ToListAsync();

                res.Message.FriendlyMessage = Messages.GetSuccess;
                res.IsSuccessful = true;
                return res;
            }

            res.IsSuccessful = true;
            return res;
        }


        async Task<APIResponse<GetClassGroupRequest>> IClassGroupService.GetSingleClassGroupsAsync(Guid groupId, Guid sessionClassId)
        {
            var res = new APIResponse<GetClassGroupRequest>();
            var student = context.StudentContact.Where(x => x.ClientId == smsClientId).Include(s => s.User).Where(e => e.SessionClassId == sessionClassId && e.EnrollmentStatus == (int)EnrollmentStatus.Enrolled).ToList();
            var result = await context.SessionClassGroup.Where(x => x.ClientId == smsClientId)
                .OrderBy(s => s.GroupName)
                .Include(d => d.SessionClass).ThenInclude(s => s.Class)
                .Include(d => d.SessionClassSubject).ThenInclude(s => s.Subject)
                .Where(d => d.Deleted == false && d.SessionClassGroupId == groupId).Select(a =>
                new GetClassGroupRequest(a, student)).FirstOrDefaultAsync();
            res.IsSuccessful = true;
            res.Result = result;
            return res;
        }

        async Task<APIResponse<MultipleDelete>> IClassGroupService.DeleteClassGroupAsync(MultipleDelete request)
        {
            var res = new APIResponse<MultipleDelete>();
            foreach(var GroupId in request.Items)
            {
                var Group = context.SessionClassGroup.FirstOrDefault(d => d.SessionClassGroupId == Guid.Parse(GroupId) && d.ClientId == smsClientId);
                if (Group == null)
                {
                    res.Message.FriendlyMessage = "Class Group does not exist";
                    return res;
                }
                context.SessionClassGroup.Remove(Group);
                await context.SaveChangesAsync();

                res.Result = request;
            }

            res.IsSuccessful = true;
            res.Message.FriendlyMessage = "You have succesfully deleted a class Group";
            return res;
        }

        public async Task<APIResponse<List<SessionClassSubjects>>> GetSessionClassSubjectsCbtAsync(Guid sessionClassId, string clientId)
        {
            var res = new APIResponse<List<SessionClassSubjects>>();
           
            res.Result = await context.SessionClassSubject.Where(x => x.ClientId == clientId)
            .Include(s => s.Subject)
            .Where(d => d.Deleted == false && d.SessionClassId == sessionClassId && d.Subject.Deleted == false && d.Subject.IsActive == true).Select(a =>
            new SessionClassSubjects(a)).ToListAsync();

            res.Message.FriendlyMessage = Messages.GetSuccess;
            res.IsSuccessful = true;
            return res;
        }

        GradeGroup IClassGroupService.GetClassGrade(Guid ClassLookupId)
        {
            return context.GradeGroup.Where(x => x.ClientId == smsClientId && x.Classes.Select(s => s.ClassLookupId).Contains(ClassLookupId)).Include(x => x.Grades).FirstOrDefault();
        }
    }
}
