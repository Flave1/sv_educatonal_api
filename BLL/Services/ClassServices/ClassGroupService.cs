using BLL.Constants;
using Contracts.Class;
using Contracts.Common;
using DAL;
using DAL.ClassEntities;
using DAL.StudentInformation;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SMP.BLL.Constants;
using SMP.BLL.Services.Constants;
using SMP.BLL.Utilities;
using SMP.Contracts.ClassModels;
using SMP.DAL.Models.ClassEntities;
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
        public ClassGroupService(DataContext context, IHttpContextAccessor accessor)
        {
            this.context = context;
            this.accessor = accessor;
        }

        async Task<APIResponse<CreateClassGroup>> IClassGroupService.CreateClassGroupAsync(CreateClassGroup request)
        {
            var res = new APIResponse<CreateClassGroup>();
            try
            {
                if (context.SessionClassGroup.AsEnumerable().Any(r => UtilTools.ReplaceWhitespace(request.GroupName) == UtilTools.ReplaceWhitespace(r.GroupName) && request.SessionClassId == r.SessionClassId.ToString()))
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
                if (context.SessionClassGroup.AsEnumerable().Any(r => UtilTools.ReplaceWhitespace(request.GroupName) == UtilTools.ReplaceWhitespace(r.GroupName) 
                && r.SessionClassGroupId != Guid.Parse(request.GroupId) && request.SessionClassId == r.SessionClassId.ToString()))
                {
                    res.Message.FriendlyMessage = "Group Name Already exist";
                    return res;
                }

                var grp = context.SessionClassGroup.FirstOrDefault(r => r.SessionClassGroupId == Guid.Parse(request.GroupId));
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
            var student = context.StudentContact.Include(s => s.User).Where(e => e.SessionClassId == sessionClassId && e.EnrollmentStatus == (int)EnrollmentStatus.Enrolled).ToList();
            
            var result = await context.SessionClassGroup
                .OrderBy(s => s.GroupName)
                .Include(d => d.SessionClass).ThenInclude(s => s.Class)
                .Include(d => d.SessionClassSubject).ThenInclude(s => s.Subject)
                .Where(d => d.Deleted == false 
                && d.GroupName != "all-students" 
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
            if (accessor.HttpContext.User.IsInRole(DefaultRoles.SCHOOLADMIN) || accessor.HttpContext.User.IsInRole(DefaultRoles.FLAVETECH))
            {
                res.Result = await context.SessionClassSubject
                .Include(s => s.Subject)
                .Where(d => d.Deleted == false && d.SessionClassId == sessionClassId).Select(a =>
                new SessionClassSubjects(a)).ToListAsync();

                res.Message.FriendlyMessage = Messages.GetSuccess;
                res.IsSuccessful = true;
                return res;
            }

            if (accessor.HttpContext.User.IsInRole(DefaultRoles.TEACHER))
            {
                var subjectTeacherSubjects = context.SessionClassSubject
                    .Include(d => d.Subject)
                    .Where(e => e.SubjectTeacherId == Guid.Parse(teacherId) && e.SessionClassId == sessionClassId).Select(s => new SessionClassSubjects(s));

                var formTeacherSubjects = context.SessionClassSubject
                    .Include(d => d.Subject)
                    .Include(d => d.SessionClass)
                    .Where(e => e.SessionClassId == sessionClassId && e.SessionClass.FormTeacherId == Guid.Parse(teacherId)).Select(s => new SessionClassSubjects(s));

                res.Result = subjectTeacherSubjects.AsEnumerable().Concat(formTeacherSubjects.AsEnumerable()).Distinct().ToList();

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
            var student = context.StudentContact.Include(s => s.User).Where(e => e.SessionClassId == sessionClassId && e.EnrollmentStatus == (int)EnrollmentStatus.Enrolled).ToList();
            var result = await context.SessionClassGroup
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
                var Group = context.SessionClassGroup.FirstOrDefault(d => d.SessionClassGroupId == Guid.Parse(GroupId));
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
   
    }
}
