﻿using BLL;
using BLL.Constants;
using BLL.StudentServices;
using BLL.Utilities;
using DAL;
using Microsoft.EntityFrameworkCore;
using SMP.BLL.Constants;
using SMP.BLL.Services.Constants;
using SMP.Contracts.PromotionModels;
using SMP.DAL.Models.PromotionEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMP.BLL.Services.PromorionServices
{
    public class PromotionService : IPromotionService
    {
        private readonly DataContext context;
        private readonly IStudentService studentService;

        public PromotionService(DataContext context, IStudentService studentService)
        {
            this.context = context;
            this.studentService = studentService;
        }

        async Task<APIResponse<List<PreviousSessionClasses>>> IPromotionService.GetPreviousSessionClassesAsync()
        {
            var res = new APIResponse<List<PreviousSessionClasses>>();

            var lastTwoSessions = await context.Session.OrderByDescending(a => a.CreatedOn).Where(d => d.Deleted  == false).Take(2).ToListAsync();

            if (lastTwoSessions.Any())
            {
                Guid previousSessionId = lastTwoSessions.Last().SessionId;

                var result = await context.SessionClass
                    .Include(rr => rr.Class)
                    .OrderBy(d => d.Class.Name)
                    .Include(d => d.Students)
                    .Where(r => r.InSession && r.Deleted == false && r.SessionId == previousSessionId)
                    .Include(rr => rr.Teacher).ThenInclude(uuu => uuu.User).Select(g => new PreviousSessionClasses(g)).ToListAsync();

                await CreateCopyOfClassesToPromoteAsync(result, previousSessionId);

                foreach(var cl in result)
                {
                    cl.IsPromoted = context.PromotedSessionClass.FirstOrDefault(d => d.SessionClassId == Guid.Parse(cl.SessionClassId)).IsPromoted;
                }

                res.Result = result;
            }
           
            res.IsSuccessful = true;
            res.Message.FriendlyMessage = Messages.GetSuccess;
            return res;
        }

        async Task<APIResponse<bool>> IPromotionService.PromoteClassAsync(Guid classToPromote, Guid classToPromoteTo)
        {
            var res = new APIResponse<bool>();

            using(var transaction = await context.Database.BeginTransactionAsync())
            {
                try
                {
                    
                    var allStudentsInClassToPromote = context.SessionClass
                        .Include(s => s.Students)
                        .FirstOrDefault(w => w.SessionClassId == classToPromote).Students;

                    if (allStudentsInClassToPromote.Any())
                    {
                        string session = Convert.ToString(allStudentsInClassToPromote.FirstOrDefault().SessionClass.SessionId);
                        foreach (var student in allStudentsInClassToPromote)
                        {
                            var enrollment = await context.Enrollment.FirstOrDefaultAsync(e => e.StudentContactId == student.StudentContactId);
                            if (enrollment != null)
                                enrollment.Status = (int)EnrollmentStatus.Enrolled;
                            await studentService.ChangeClassAsync(student.StudentContactId, classToPromoteTo);

                        }
                        await context.SaveChangesAsync();
                        //return null;
                        await UpdatePromotedClassAsync(classToPromote, session);
                        await transaction.CommitAsync();
                        res.Message.FriendlyMessage = "Promotion Successful";
                        res.Result = true;
                        res.IsSuccessful = true;
                    }
                    else
                    {
                        await transaction.RollbackAsync();
                        res.Message.FriendlyMessage = "No student found in this class to promote";
                    }
                    return res;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    res.Message.FriendlyMessage = Messages.ClassTransitionException;
                    res.Message.TechnicalMessage = ex.ToString();
                    return res;
                }
            }
           
        }

        
        async Task<APIResponse<List<GetStudents>>> IPromotionService.GetAllPassedStudentsAsync(Guid SessionClassId)
        {
            var res = new APIResponse<List<GetStudents>>();
            var regNoFormat = RegistrationNumber.config.GetSection("RegNumber:Student").Value;

            var result = await context.StudentContact
                .OrderByDescending(d => d.CreatedOn)
                .OrderByDescending(s => s.RegistrationNumber)
                .Include(q => q.SessionClass).ThenInclude(s => s.Class)
                .Include(q => q.User)
                .Where(d => d.Deleted == false && d.SessionClassId == SessionClassId && d.SessionClass.PassMark > 0)
                .Select(f => new GetStudents(f, "passed", regNoFormat)).ToListAsync();

            res.Message.FriendlyMessage = Messages.GetSuccess;
            res.Result = result;
            res.IsSuccessful = true;
            return res;
        }

        async Task<APIResponse<List<GetStudents>>> IPromotionService.GetAllFailedStudentsAsync(Guid SessionClassId)
        {
            var res = new APIResponse<List<GetStudents>>();
            var regNoFormat = RegistrationNumber.config.GetSection("RegNumber:Student").Value;

            var result = await context.StudentContact
                .OrderByDescending(d => d.CreatedOn)
                .OrderByDescending(s => s.RegistrationNumber)
                .Include(q => q.SessionClass).ThenInclude(s => s.Class)
                .Include(q => q.User)
                .Where(d => d.Deleted == false && d.SessionClassId == SessionClassId)
                .Select(f => new GetStudents(f, "failed", regNoFormat)).ToListAsync();

            res.Message.FriendlyMessage = Messages.GetSuccess;
            res.Result = result;
            res.IsSuccessful = true;
            return res;
        }

        async Task CreateCopyOfClassesToPromoteAsync(List<PreviousSessionClasses> prevClasses, Guid previousSessionId)
        {
            if (await context.PromotedSessionClass.CountAsync(d => d.SessionId == previousSessionId) == prevClasses.Count())
            {
                return;
            }
            else
            {
                foreach (var prevClass in prevClasses)
                {
                    var classToPrommote = await context.PromotedSessionClass.FirstOrDefaultAsync(d => d.SessionId == previousSessionId && d.SessionClassId == Guid.Parse(prevClass.SessionClassId));
                    if (classToPrommote == null)
                    {
                        classToPrommote = new PromotedSessionClass();
                        classToPrommote.SessionClassId = Guid.Parse(prevClass.SessionClassId);
                        classToPrommote.SessionId = previousSessionId;
                        context.PromotedSessionClass.Add(classToPrommote);
                        await context.SaveChangesAsync();
                    }
                }
            }
        }
        async Task UpdatePromotedClassAsync(Guid sessionClassId, string previousSessionId)
        {
            var classToPrommote = await context.PromotedSessionClass.FirstOrDefaultAsync(d => d.SessionId == Guid.Parse(previousSessionId) && d.SessionClassId == sessionClassId);
            if (classToPrommote != null)
            {
                classToPrommote.IsPromoted = true;
                await context.SaveChangesAsync();
            }
        }
    }
}