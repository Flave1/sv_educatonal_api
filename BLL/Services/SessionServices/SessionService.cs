﻿using BLL.Filter;
using BLL.LoggerService;
using BLL.Wrappers;
using Contracts.Session;
using DAL;
using DAL.ClassEntities;
using DAL.SessionEntities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SMP.BLL.Constants;
using SMP.BLL.Services.FilterService;
using SMP.BLL.Services.ResultServices;
using SMP.BLL.Services.SessionServices;
using SMP.DAL.Models.ClassEntities;
using SMP.DAL.Models.SessionEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GetSessionClass = Contracts.Session.GetSessionClass;

namespace BLL.SessionServices
{
    public class SessionService : ISessionService
    {
        private readonly DataContext context;
        private readonly IResultsService resultsService;
        private readonly IPaginationService paginationService;
        private readonly ILoggerService loggerService;
        private readonly string smsClientId;
        private readonly ITermService termService;

        public SessionService(DataContext context, IResultsService resultsService, IPaginationService paginationService, IHttpContextAccessor accessor, ILoggerService loggerService, ITermService termService)
        {
            this.context = context;
            this.resultsService = resultsService;
            this.paginationService = paginationService;
            this.loggerService = loggerService;
            smsClientId = accessor.HttpContext.User.FindFirst(x => x.Type == "smsClientId")?.Value;
            this.termService = termService;
        }
        async Task<APIResponse<Session>> ISessionService.SwitchSessionAsync(string sessionId)
        {
            var res = new APIResponse<Session>();

            if (!await resultsService.AllResultPublishedAsync())
            {
                res.Message.FriendlyMessage = $"Ensure all class results for current session are published";
                return res;
            }
            var savedSession = context.Session
                .Where(x => x.ClientId == smsClientId && x.SessionId == Guid.Parse(sessionId))
                .Include(s => s.Terms).FirstOrDefault();
            if (savedSession == null)
            {
                res.Message.FriendlyMessage = $"Session Not Found";
                return res;
            }

            await SetOtherSessionsInactiveAsync(Guid.Parse(sessionId));

            savedSession.IsActive = true;
            await termService.SetFirstTermActiveAsync(savedSession.SessionId);
            await context.SaveChangesAsync();
            var message = $"Successfuly switched to {savedSession.StartDate} / {savedSession.EndDate} session";
            res.Result = savedSession;
            res.Message.FriendlyMessage = message;
            res.IsSuccessful = true;
            return res;
        }
        async Task<APIResponse<CreateUpdateSession>> ISessionService.CreateSessionAsync(CreateUpdateSession session)
        {
            var res = new APIResponse<CreateUpdateSession>();

            if (!await resultsService.AllResultPublishedAsync())
            {
                res.Message.FriendlyMessage = $"Ensure all class results for current session are published";
                return res;
            }

            if (context.Session.Any(s => s.ClientId == smsClientId && s.StartDate.Trim().ToLower() == session.StartDate.ToLower() && s.EndDate.Trim().ToLower() == session.EndDate.ToLower()))
            {
                res.Message.FriendlyMessage = $"Session {session.StartDate} - {session.EndDate} is already created";
                return res;
            }

            
            var currentSession = await context.Session.Where(x=>x.ClientId == smsClientId && x.IsActive == true).FirstOrDefaultAsync();

            try
            {
                var dbSession = new Session
                {
                    StartDate = session.StartDate,
                    EndDate = session.EndDate,
                    IsActive = true,
                    HeadTeacherId = Guid.Parse(session.HeadTeacherId),
                };
                context.Session.Add(dbSession);
                await context.SaveChangesAsync();

                await SetOtherSessionsInactiveAsync(dbSession.SessionId);

                await termService.CreateSessionTermsAsync(dbSession.SessionId, session.Terms);

                if (session.TransferClasses && !IsAnewSchool())
                {
                    await TransferSessionRecord(currentSession, dbSession);
                }

                res.IsSuccessful = true;
                res.Result = session;
                res.Message.FriendlyMessage = $"Successfuly created a session with {session.Terms} terms";
                return res;
            }
            catch (Exception ex)
            {
                loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                res.Message.FriendlyMessage = "Error Occurred!! Please contact administrator";
                res.Message.TechnicalMessage = ex?.Message ?? ex.InnerException.ToString();
                return res;
            }

        }

       
        private bool IsAnewSchool()
        {
            var allClasses =  context.SessionClass
                .Where(x => x.ClientId == smsClientId)
                .Where(d => d.Deleted == false)
                .ToList();
            if (!allClasses.Any())
                return true;
            return false;
        }

       

        private async Task SetOtherSessionsInactiveAsync(Guid currentSessionId)
        {
            var sessions = await context.Session.Where(x => x.ClientId == smsClientId)
                .Where(er => er.SessionId != currentSessionId && er.IsActive == true)
                .Include(sd => sd.Terms)
                .ToListAsync();

            foreach(var session in sessions)
            {
                session.IsActive = false;
                foreach(var term in session.Terms)
                {
                    term.IsActive = false;
                }
            }
            await context.SaveChangesAsync();

        }


        async Task<APIResponse<Session>> ISessionService.DeleteSessionAsync(Guid sessionId)
        {
            var res = new APIResponse<Session>();
            var savedSession = context.Session.Where(x => x.ClientId == smsClientId).Include(x => x.SessionClass).FirstOrDefault(d => d.SessionId == sessionId);
            
            if (savedSession != null)
            {
                if (savedSession.IsActive)
                {
                    res.Message.FriendlyMessage = $"Active session can not be deleted";
                    return res;
                }
                if (savedSession.SessionClass.Any(x => x.Deleted == false))
                {
                    res.Message.FriendlyMessage = $"Session with classes cannot be deleted";
                    return res;
                }

                savedSession.Deleted = true;
                savedSession.StartDate = savedSession.StartDate + "_DELETED";
                savedSession.EndDate = savedSession.EndDate + "_DELETED";
                await context.SaveChangesAsync();
            }
            res.IsSuccessful = true;
            res.Result = savedSession;
            res.Message.FriendlyMessage = $"Successfuly deleted session";
            return res;
        }

        async Task<APIResponse<PagedResponse<List<GetSession>>>> ISessionService.GetSessionsAsync(PaginationFilter filter)
        {
            var res = new APIResponse<PagedResponse<List<GetSession>>>();
            var query =  context.Session.Where(x => x.ClientId == smsClientId)
                .Include(er => er.HeadTeacher)
                .OrderByDescending(d => d.StartDate).Where(d => d.Deleted == false);


            var totaltRecord = query.Count();
            var result = await paginationService.GetPagedResult(query, filter).Select(e => new GetSession
            {
                EndDate = e.EndDate,
                SessionId = e.SessionId.ToString(),
                StartDate = e.StartDate,
                IsActive = e.IsActive,
                HeadTeacherId = e.HeadTeacherId,
                HeadTeacherName = e.HeadTeacher.FirstName + " " + e.HeadTeacher.LastName,
                Terms = e.Terms.OrderBy(s => s.TermName).Select(t => new Terms
                {
                    IsActive = t.IsActive,
                    SessionTermId = t.SessionTermId,
                    TermName = t.TermName,
                }).ToArray()
            }
                ).ToListAsync();
            res.Result = paginationService.CreatePagedReponse(result, filter, totaltRecord);

            res.IsSuccessful = true;
            return res;
        }

        async Task<APIResponse<GetSession>> ISessionService.GetSingleSessionAsync(string sessionId)
        {
            var res = new APIResponse<GetSession>();
            var session = context.Session.Where(x => x.ClientId == smsClientId && x.Deleted == false && Guid.Parse(sessionId) == x.SessionId);


            if (session.FirstOrDefault().IsActive)
            {
                res.Result = await session
                 .Include(d => d.SessionClass).ThenInclude(d => d.Students)
                 .Include(d => d.SessionClass).ThenInclude(d => d.Teacher).ThenInclude(d => d.User)
                 .Include(er => er.HeadTeacher)
                 .OrderByDescending(d => d.CreatedOn)
                 .Select(e => new GetSession
                 {
                     EndDate = e.EndDate,
                     SessionId = e.SessionId.ToString(),
                     StartDate = e.StartDate,
                     IsActive = e.IsActive,
                     HeadTeacherId = e.HeadTeacherId,
                     HeadTeacherName = e.HeadTeacher.FirstName + " " + e.HeadTeacher.LastName,
                     Terms = e.Terms.OrderByDescending(s => s.TermName).Select(t => new Terms
                     {
                         IsActive = t.IsActive,
                         SessionTermId = t.SessionTermId,
                         TermName = t.TermName,
                     }).ToArray(),
                     SessionClasses = e.SessionClass.Select(d => new GetSessionClass
                     {
                         FormTeacher = d.Teacher.FirstName + " " + d.Teacher.LastName,
                         SessionClass = d.Class.Name,
                         SessionClassId = d.SessionClassId
                     }).ToArray(),
                     NoOfStudents = e.SessionClass.SelectMany(s => s.Students).Count(),
                     NoOfClasses = e.SessionClass.Count(),
                     NoOfSubjects = e.SessionClass.SelectMany(s => s.SessionClassSubjects).Count(),
                 }
                 ).FirstOrDefaultAsync();
            }
            else
            {
                var result = await session
                .Include(d => d.SessionClass)
                .OrderByDescending(d => d.CreatedOn)
                .Select(e => new GetSession
                {
                    EndDate = e.EndDate,
                    SessionId = e.SessionId.ToString(),
                    StartDate = e.StartDate,
                    IsActive = e.IsActive,
                    HeadTeacherId = e.HeadTeacherId,
                    HeadTeacherName = e.HeadTeacher.FirstName + " " + e.HeadTeacher.LastName,

                }).FirstOrDefaultAsync();

                if(result != null)
                {
                    result.Terms = termService.GetTermsBySession(Guid.Parse(result.SessionId)).Select(m=> new Terms
                    {
                        SessionTermId = m.SessionTermId,
                        TermName = m.TermName,
                        IsActive = m.IsActive,
                    }).ToArray();

                    result.SessionClasses = context.SessionClass.Where(x => x.SessionId == Guid.Parse(result.SessionId)).Select(d => new GetSessionClass
                    {
                        FormTeacher = d.Teacher.FirstName + " " + d.Teacher.LastName,
                        SessionClass = d.Class.Name,
                        SessionClassId = d.SessionClassId
                    }).ToArray();

                    result.NoOfStudents = context.StudentSessionClassHistory.Where(x => result.SessionClasses.Select(c=>c.SessionClassId).Contains(x.SessionClassId))
                                          .Select(s => s.StudentContactId).Distinct().Count();
                    result.NoOfClasses = result.SessionClasses.Count();
                    result.NoOfSubjects = context.SessionClass.Where(x => x.SessionId == Guid.Parse(result.SessionId)).SelectMany(s => s.SessionClassSubjects).Count();
                }

                res.Result = result;

            }
            

            res.IsSuccessful = true;
            return res;
        }

        

        
        async Task<APIResponse<ActiveSession>> ISessionService.GetActiveSessionsAsync()
        {
            var res = new APIResponse<ActiveSession>();
            var result = await context.Session.Where(d => d.Deleted == false && d.IsActive == true && d.ClientId == smsClientId).Include(d => d.Terms).Select(d => new ActiveSession
            {
                SessionId = d.SessionId,
                Session = d.StartDate + " / " + d.EndDate,
                SessionTermId = d.Terms.FirstOrDefault(er => er.IsActive == true).SessionTermId,
                SessionTerm = d.Terms.FirstOrDefault(er => er.IsActive == true).TermName,
                Terms = d.Terms.OrderBy(s => s.TermName).Select(t => new Terms
                {
                    IsActive = t.IsActive,
                    SessionTermId = t.SessionTermId,
                    TermName = t.TermName,
                }).ToArray(),
            }).FirstOrDefaultAsync();

            
                 
            res.IsSuccessful = true;
            res.Result = result;
            return res;
        }

       

        async Task<APIResponse<bool>> ISessionService.UpdateSessionHeadTeacherAsync(UpdateHeadTeacher req)
        {
            var res = new APIResponse<bool>();
            try
            {
                var savedSession = context.Session.FirstOrDefault(d => d.ClientId == smsClientId && d.SessionId == Guid.Parse(req.SessionId));

                if (savedSession != null)
                {
                    if (!savedSession.IsActive)
                    {
                        res.Message.FriendlyMessage = "This Session cannot be edited";
                        return res;
                    }
                    savedSession.HeadTeacherId = Guid.Parse(req.HeadTeacherId);
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                res.Message.FriendlyMessage = Messages.Updated;
                res.Message.TechnicalMessage = ex?.Message ?? ex?.InnerException.ToString();
                return res;
            }
            res.Result=true;
            res.IsSuccessful = true;
            res.Message.FriendlyMessage = Messages.Updated;
            return res;
        }

        async Task<APIResponse<List<GetSessionClass>>> ISessionService.GetSessionClassesAsync(Guid sessionId)
        {
            var res = new APIResponse<List<GetSessionClass>>();
            var result = await context.SessionClass.Where(r => r.ClientId == smsClientId && r.Deleted == false && r.SessionId == sessionId)
                .Include(rr => rr.Class)
                .OrderBy(d => d.Class.Name)
                .Select(g => new GetSessionClass {
                    SessionClass = g.Class.Name,
                    SessionClassId = g.SessionClassId
                }).ToListAsync();
            res.IsSuccessful = true;
            res.Result = result;
            return res;
        }

        SessionTerm ISessionService.GetPreviousSessionLastTermAsync(Guid sessionId)
        {
            return context.Session.Where(x => x.ClientId == smsClientId).Include(s => s.Terms)
                                .FirstOrDefault(e => e.SessionId == sessionId).Terms
                                .OrderBy(d => d.TermName).LastOrDefault();
        }

        public async Task<APIResponse<ActiveSessionCbt>> GetActiveSessionsCbtAsync(int examScore, bool asExamScore, bool asAssessmentScore, string clientId)
        {
            var res = new APIResponse<ActiveSessionCbt>();
            try
            {
                var result = await context.Session.Where(x => x.ClientId == clientId).Include(d => d.Terms).Where(d => d.Deleted == false && d.IsActive == true).Select(d => new ActiveSessionCbt
                {
                    SessionId = d.SessionId.ToString(),
                    Session = d.StartDate + " / " + d.EndDate,
                    SessionTermId = d.Terms.FirstOrDefault(er => er.IsActive == true).SessionTermId.ToString(),
                    SessionTerm = d.Terms.FirstOrDefault(er => er.IsActive == true).TermName,
                }).FirstOrDefaultAsync() ?? null;

                var sessionClass = await context.SessionClass.FirstOrDefaultAsync(x => x.SessionId == Guid.Parse(result.SessionId) && x.ClientId == clientId);

                if(asExamScore)
                {
                    if (examScore > sessionClass.ExamScore)
                    {
                        res.IsSuccessful = false;
                        res.Message.FriendlyMessage = "Exam Score cannot be greater than Session Class Exam Score";
                        return res;
                    }
                }

                if(asAssessmentScore)
                {
                    if (examScore > sessionClass.AssessmentScore)
                    {
                        res.IsSuccessful = false;
                        res.Message.FriendlyMessage = "Exam Score cannot be greater than Session Class Assessment Score";
                        return res;
                    }
                }

                res.IsSuccessful = true;
                res.Message.FriendlyMessage = Messages.GetSuccess;
                res.Result = result;
                return res;
            }
            catch(Exception ex)
            {
                loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                res.IsSuccessful = false;
                res.Message.FriendlyMessage = Messages.FriendlyException;
                return res;
            }
        }

        private async Task TransferSessionRecord(Session previousSession, Session newSession)
        {
            try
            {
                var sessionClasses = await context.SessionClass.Where(x => x.ClientId == smsClientId && x.SessionId == previousSession.SessionId).ToListAsync();

                for(int i = 0; i < sessionClasses.Count; i++ )
                {
                    var newSessionClass = new SessionClass
                    {
                        ClassId = sessionClasses[i].ClassId,
                        FormTeacherId = sessionClasses[i].FormTeacherId,
                        SessionId = newSession.SessionId,
                        InSession = sessionClasses[i].InSession,
                        ExamScore = sessionClasses[i].ExamScore,
                        AssessmentScore = sessionClasses[i].AssessmentScore,
                        PassMark = sessionClasses[i].PassMark,
                        SessionTermId = termService.GetCurrentTerm().SessionTermId
                    };
                    context.SessionClass.Add(newSessionClass);
                    await context.SaveChangesAsync();
                    await TransferSessionClassSubject(sessionClasses[i].SessionClassId, newSessionClass.SessionClassId);

                }
            }
            catch(Exception ex)
            {
                loggerService.Error(ex?.Message, ex?.StackTrace, ex?.InnerException?.ToString(), ex?.InnerException?.Message);
                throw;
            }
            
        }

        private async Task TransferSessionClassSubject(Guid previousSessionClassId, Guid newSessionClassId)
        {
            var sessionClassSubjects = await context.SessionClassSubject.Where(x => x.SessionClassId == previousSessionClassId && x.ClientId == smsClientId).ToListAsync();

            await CreateUpdateClassSubjectsAsync(sessionClassSubjects, newSessionClassId);
        }

        private async Task CreateUpdateClassSubjectsAsync(List<SessionClassSubject> sessionClassSubjects, Guid newSessionClassId)
        {
            for (int i = 0; i<sessionClassSubjects.Count; i++)
            {
                var sub = new SessionClassSubject();
                sub.SessionClassId = newSessionClassId;
                sub.SubjectId = sessionClassSubjects[i].SubjectId;
                sub.SubjectTeacherId = sessionClassSubjects[i].SubjectTeacherId;
                sub.AssessmentScore = sessionClassSubjects[i].AssessmentScore;
                sub.ExamScore = sessionClassSubjects[i].ExamScore;
                await context.SessionClassSubject.AddAsync(sub);
               
            }
            await context.SaveChangesAsync();
        }

       
    }
}
