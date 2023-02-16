using BLL.Filter;
using BLL.Wrappers;
using Contracts.Session;
using DAL;
using DAL.SessionEntities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SMP.BLL.Constants;
using SMP.BLL.Services.FilterService;
using SMP.BLL.Services.ResultServices;
using SMP.BLL.Utilities;
using SMP.DAL.Models.SessionEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BLL.SessionServices
{
    public class SessionService : ISessionService
    {
        private readonly DataContext context;
        private readonly IResultsService resultsService;
        private readonly IPaginationService paginationService;
        private readonly string smsClientId;

        public SessionService(DataContext context, IResultsService resultsService, IPaginationService paginationService, IHttpContextAccessor accessor)
        {
            this.context = context;
            this.resultsService = resultsService;
            this.paginationService = paginationService;
            smsClientId = accessor.HttpContext.User.FindFirst(x => x.Type == "smsClientId")?.Value;
        }
        async Task<APIResponse<Session>> ISessionService.SwitchSessionAsync(string sessionId)
        {
            var res = new APIResponse<Session>();
            var savedSession = context.Session.Where(x => x.ClientId == smsClientId).Include(s => s.Terms).FirstOrDefault(d => d.SessionId == Guid.Parse(sessionId));

            if (!await resultsService.AllResultPublishedAsync())
            {
                res.Message.FriendlyMessage = $"Ensure all class results for current session are published";
                return res;
            }
            if (savedSession == null)
            {
                res.Message.FriendlyMessage = $"Session Not Found";
                return res;
            }

            await SetOtherSessionsInactiveAsync(Guid.Parse(sessionId));

            savedSession.IsActive = true;
            savedSession.Terms.FirstOrDefault().IsActive = true;
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

            using(var transaction = await context.Database.BeginTransactionAsync())
            {
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

                    await CreateSessionTermsAsync(dbSession.SessionId, session.Terms);

                    await transaction.CommitAsync();
                    res.IsSuccessful = true;
                    res.Result = session;
                    res.Message.FriendlyMessage = $"Successfuly created a session with {session.Terms} terms";
                    return res;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    res.Message.FriendlyMessage = "Error Occurred!! Please contact administrator";
                    res.Message.TechnicalMessage = ex?.Message ?? ex.InnerException.ToString();
                    return res;
                }
                finally { await transaction.DisposeAsync(); }
            }
           

        }

        private async Task CreateSessionTermsAsync(Guid sessionId, int noOfTerms)
        {
            noOfTerms += 1;
            for (var i = 1; i < noOfTerms; i++)
            {
                var termName = Tools.OrdinalSuffixOf(i);

                var term = new SessionTerm
                {
                    IsActive = i == 1 ? true : false,
                    TermName = termName,
                    SessionId = sessionId,
                };
                context.SessionTerm.Add(term);
                await context.SaveChangesAsync();
            }
        }

       

        private async Task SetOtherSessionsInactiveAsync(Guid currentSessionId)
        {
            var sessions = await context.Session.Where(x => x.ClientId == smsClientId).Include(sd => sd.Terms).Where(er => er.SessionId != currentSessionId && er.IsActive == true).ToListAsync();
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
            var result = await context.Session.Where(x => x.ClientId == smsClientId && x.Deleted == false && Guid.Parse(sessionId) == x.SessionId)
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

            res.IsSuccessful = true;
            res.Result = result;
            return res;
        }


        async Task<APIResponse<bool>> ISessionService.ActivateTermAsync(Guid termId)
        {
            var res = new APIResponse<bool>();
            //if (!await resultsService.AllResultPublishedAsync())
            //{
            //    res.Message.FriendlyMessage = $"Ensure all class results for current session are published";
            //    return res;
            //}
            var term = await context.SessionTerm.FirstOrDefaultAsync(st => st.SessionTermId == termId && st.ClientId == smsClientId);
            if (term == null)
            {
                res.Message.FriendlyMessage = "Session not found";
                return res;
            }
            term.IsActive = true;
            var otherTerms = await context.SessionTerm.Where(st => st.SessionId == term.SessionId && st.SessionTermId != term.SessionTermId && st.ClientId == smsClientId).ToListAsync();
            if (otherTerms.Any())
            {
                foreach(var otherTerm in otherTerms)
                {
                    otherTerm.IsActive = false;
                }
            }
            await context.SaveChangesAsync();
            res.Result = true;
            res.IsSuccessful = true;
            res.Message.FriendlyMessage = $"Successfuly activated {term.TermName} term";
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
                res.Message.FriendlyMessage = Messages.Updated;
                res.Message.TechnicalMessage = ex?.Message ?? ex?.InnerException.ToString();
                return res;
            }
            res.Result=true;
            res.IsSuccessful = true;
            res.Message.FriendlyMessage = Messages.Updated;
            return res;
        }


        async Task<APIResponse<List<Terms>>> ISessionService.GetSessionTermsAsync(Guid sessionId)
        {
            var res = new APIResponse<List<Terms>>();
            var result = await context.SessionTerm.Where(d => d.ClientId == smsClientId && d.SessionId == sessionId).Select(t => new Terms
            {
                IsActive = t.IsActive,
                SessionTermId = t.SessionTermId,
                TermName = t.TermName,
            }).ToListAsync();

            res.IsSuccessful = true;
            res.Result = result;
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

        public async Task<APIResponse<ActiveSessionCbt>> GetActiveSessionsCbtAsync(int examScore, bool asExamScore, bool asAssessmentScore)
        {
            var res = new APIResponse<ActiveSessionCbt>();
            try
            {
                var result = await context.Session.Where(x => x.ClientId == smsClientId).Include(d => d.Terms).Where(d => d.Deleted == false && d.IsActive == true).Select(d => new ActiveSessionCbt
                {
                    SessionId = d.SessionId.ToString(),
                    Session = d.StartDate + " / " + d.EndDate,
                    SessionTermId = d.Terms.FirstOrDefault(er => er.IsActive == true).SessionTermId.ToString(),
                    SessionTerm = d.Terms.FirstOrDefault(er => er.IsActive == true).TermName,
                }).FirstOrDefaultAsync() ?? null;

                var sessionClass = await context.SessionClass.FirstOrDefaultAsync(x => x.SessionId == Guid.Parse(result.SessionId) && x.ClientId == smsClientId);

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
                res.IsSuccessful = false;
                res.Message.FriendlyMessage = Messages.FriendlyException;
                return res;
            }
        }
    }
}
