﻿using BLL;
using DAL;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SMP.BLL.Constants;
using SMP.Contracts.Assessment;
using SMP.DAL.Models.AssessmentEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SMP.BLL.Services.AssessmentServices
{
    public class ClassAssessmentService : IClassAssessmentService
    {
        private readonly DataContext context;
        private readonly IHttpContextAccessor accessor;
        public ClassAssessmentService(DataContext context, IHttpContextAccessor accessor)
        {
            this.context = context;
            this.accessor = accessor;
        }

        async Task<APIResponse<List<GetClassAssessmentRequest>>> IClassAssessmentService.GetAssessmentByTeacherAsync()
        {
            var teacherId = accessor.HttpContext.User.FindFirst(e => e.Type == "teacherId")?.Value;
            var res = new APIResponse<List<GetClassAssessmentRequest>>();
            
            res.Result = await context.ClassAssessment
                 .Include(s => s.SessionClassSubject)
                 .Include(s => s.SessionClass).ThenInclude(c => c.Class)
                 .Include(x => x.SessionClassSubject).ThenInclude(d => d.Subject)
                 .Include(x => x.SessionClass).ThenInclude(d => d.Students).ThenInclude(d => d.User)
                 .Where(x => x.Scorer == Guid.Parse(teacherId)).Select(s => new GetClassAssessmentRequest(s)).ToListAsync();

            res.IsSuccessful = true;
            return await Task.Run(() => res);
        }


        async Task<APIResponse<GetClassAssessmentRequest>> IClassAssessmentService.CreateClassAssessmentAsync(CreateClassAssessment request)
        {
            var res = new APIResponse<GetClassAssessmentRequest>();
            try
            {
                var teacherId = accessor.HttpContext.User.FindFirst(e => e.Type == "teacherId")?.Value;

                var classSubject = await context.SessionClassSubject
                    .Include(d => d.SessionClass).ThenInclude(s => s.Class)
                    .Include(d => d.Subject).FirstOrDefaultAsync(d => d.SessionClassSubjectId == Guid.Parse(request.SessionClassSubjectId));
                if(classSubject is null)
                {
                    res.Message.FriendlyMessage = Messages.FriendlyNOTFOUND;
                    return res;
                }

                var classAssessment = new ClassAssessment();
                classAssessment.SessionClassSubjectId = classSubject.SessionClassSubjectId;
                classAssessment.Description = $"Class Assessment on {classSubject.Subject.Name}";
                classAssessment.AssessmentScore = 0;
                classAssessment.SessionClassId = classSubject.SessionClassId;
                classAssessment.ListOfStudentIds = "";
                classAssessment.Scorer = Guid.Parse(teacherId);
                context.ClassAssessment.Add(classAssessment);

                await context.SaveChangesAsync();
                res.Result = new GetClassAssessmentRequest(classAssessment, classSubject);
                res.IsSuccessful = true;
                res.Message.FriendlyMessage = Messages.Created;
                return res;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        async Task<APIResponse<List<ClassAssessmentStudents>>> IClassAssessmentService.GetClassStudentByAssessmentAsync(Guid classAssessmentId)
        {
            var res = new APIResponse<List<ClassAssessmentStudents>>();
            res.Result = new List<ClassAssessmentStudents>();
            var ass = context.ClassAssessment
                .Include(s => s.SessionClassSubject).ThenInclude(d => d.SessionClassGroups)
                .Include(x => x.SessionClass).ThenInclude(d => d.Students).ThenInclude(d => d.User)
                .FirstOrDefault(x => x.ClassAssessmentId == classAssessmentId);

            if (ass is null)
            {
                res.Message.FriendlyMessage = Messages.FriendlyNOTFOUND;
                return res;
            }

            var students = ass.SessionClass.Students.ToList();
            foreach(var st in students)
            {
                var item = new ClassAssessmentStudents();
                item.StudentName =  st.User.FirstName + " " + st.User.MiddleName + " " + st.User.LastName;
                item.Score = context.AssessmentScoreRecord.FirstOrDefault(d => d.AssessmentType == (int)AssessmentTypes.ClassAssessment && classAssessmentId == d.ClassAssessmentId)?.Score ?? 0;
                item.GroupIds = ass.SessionClass.SessionClassSubjects.SelectMany(d => d.SessionClassGroups).Select(d => d.SessionClassGroupId).Distinct().ToArray();
                item.StudentContactId = st.StudentContactId.ToString();
                res.Result.Add(item);
            }
            res.IsSuccessful = true;
            return await Task.Run(() => res);
        }

    
        async Task<APIResponse<UpdatetudentAssessmentScore>> IClassAssessmentService.UpdateStudentAssessmentScoreAsync(UpdatetudentAssessmentScore request)
        {
            var res = new APIResponse<UpdatetudentAssessmentScore>();
            try
            {
                var ass = context.ClassAssessment.FirstOrDefault(d => d.ClassAssessmentId == Guid.Parse(request.ClassAssessmentId));

                if(ass is null)
                {
                    res.Message.FriendlyMessage = Messages.FriendlyNOTFOUND;
                    return res;
                }

                if (request.Score > ass.AssessmentScore)
                {
                    res.Message.FriendlyMessage = $"Student assessment can not be scored more than {ass.AssessmentScore}";
                    return res;
                }

                var score = await context.AssessmentScoreRecord.FirstOrDefaultAsync(d => d.HomeAssessmentId == ass.ClassAssessmentId && d.StudentContactId == Guid.Parse(request.StudentContactId));

                if (score is null)
                {
                    score = new AssessmentScoreRecord
                    {
                        AssessmentType = (int)AssessmentTypes.ClassAssessment,
                        Score = request.Score,
                        StudentContactId = Guid.Parse(request.StudentContactId),
                        HomeAssessmentId = ass.ClassAssessmentId,
                    };
                    context.AssessmentScoreRecord.Add(score);
                }
                else
                {
                    score.Score = request.Score;
                }

                await context.SaveChangesAsync();
                res.Result = request;
                res.IsSuccessful = true;
                res.Message.FriendlyMessage = Messages.Saved;
                return res;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
