using Contracts.Session;
using DAL;
using DAL.SessionEntities;
using Microsoft.EntityFrameworkCore;
using SMP.BLL.Constants;
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

        public SessionService(DataContext context)
        {
            this.context = context;
        }
        async Task<APIResponse<Session>> ISessionService.SwitchSessionAsync(string sessionId, bool switchValue)
        {
            var res = new APIResponse<Session>();
            var savedSession = context.Session.FirstOrDefault(d => d.SessionId == Guid.Parse(sessionId));
            if (savedSession == null)
            {
                res.Message.FriendlyMessage = $"Session Not Found";
                return res;
            }

            if(switchValue && context.Session.Any(e => e.IsActive && e.SessionId != Guid.Parse(sessionId)))
            {
                res.Message.FriendlyMessage = $"Running Session Detected";
                return res;
            }

            await SetOtherSessionsInactiveAsync(Guid.Parse(sessionId));

            savedSession.IsActive = switchValue; 
            await context.SaveChangesAsync();
            var message = !switchValue ? "Successfuly switched off session" : "Successfuly switched on session";
            res.Result = savedSession;
            res.Message.FriendlyMessage = message;
            return res;
        }
        async Task<APIResponse<CreateUpdateSession>> ISessionService.CreateSessionAsync(CreateUpdateSession session)
        {
            var res = new APIResponse<CreateUpdateSession>();

            if(context.Session.Any(s => s.StartDate.Trim().ToLower() == session.StartDate.ToLower() && s.EndDate.Trim().ToLower() == session.EndDate.ToLower()))
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
                string termName = "";

                termName = OrdinalSuffixOf(i);

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

        private string OrdinalSuffixOf(int i)
        {
            var j = i % 10;
            var k = i % 100;
            if (j == 1 && k != 11)
            {
                return i + "st";
            }
            if (j == 2 && k != 12)
            {
                return i + "nd";
            }
            if (j == 3 && k != 13)
            {
                return i + "rd";
            }
            return i + "th";
        }

        private async Task SetOtherSessionsInactiveAsync(Guid currentSessionId)
        {
            var sessions = await context.Session.Include(sd => sd.Terms).Where(er => er.SessionId != currentSessionId && er.IsActive == true).ToListAsync();
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
            var savedSession = context.Session.FirstOrDefault(d => d.SessionId == sessionId);
            
            if (savedSession != null)
            {
                if (savedSession.IsActive)
                {
                    res.Message.FriendlyMessage = $"Active session can not be deleted";
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

        async Task<APIResponse<List<GetSession>>> ISessionService.GetSessionsAsync()
        {
            var res = new APIResponse<List<GetSession>>();
            var result = await context.Session.Include(er => er.HeadTeacher).ThenInclude(er => er.User).OrderByDescending(d => d.StartDate).Where(d => d.Deleted == false)
                .Select(e => new GetSession { 
                    EndDate = e.EndDate, 
                    SessionId = e.SessionId.ToString(), 
                    StartDate = e.StartDate,
                    IsActive = e.IsActive,
                    HeadTeacherId = e.HeadTeacherId,
                    HeadTeacherName = e.HeadTeacher.User.FirstName + " " + e.HeadTeacher.User.LastName,
                    Terms = e.Terms.OrderBy(s => s.TermName).Select(t => new Terms
                    {
                        IsActive = t.IsActive,
                        SessionTermId = t.SessionTermId,
                        TermName = t.TermName,
                    }).ToArray()
                }
                ).ToListAsync();

            res.IsSuccessful = true;
            res.Result = result;
            return res;
        }

        async Task<APIResponse<GetSession>> ISessionService.GetSingleSessionAsync(string sessionId)
        {
            var res = new APIResponse<GetSession>();
            var result = await context.Session
                .Include(d => d.SessionClass).ThenInclude(d => d.Students)
                .Include(d => d.SessionClass).ThenInclude(d => d.Teacher).ThenInclude(d => d.User)
                .Include(er => er.HeadTeacher).ThenInclude(er => er.User)
                .OrderByDescending(d => d.CreatedOn).Where(d => d.Deleted == false && Guid.Parse(sessionId) == d.SessionId)
                .Select(e => new GetSession
                {
                    EndDate = e.EndDate,
                    SessionId = e.SessionId.ToString(),
                    StartDate = e.StartDate,
                    IsActive = e.IsActive,
                    HeadTeacherId = e.HeadTeacherId,
                    HeadTeacherName = e.HeadTeacher.User.FirstName + " " + e.HeadTeacher.User.LastName,
                    Terms = e.Terms.OrderByDescending(s => s.TermName).Select(t => new Terms
                    {
                        IsActive = t.IsActive,
                        SessionTermId = t.SessionTermId,
                        TermName = t.TermName,
                    }).ToArray(),
                    SessionClasses = e.SessionClass.Select(d => new GetSessionClass
                    {
                        FormTeacher = d.Teacher.User.FirstName + " " + d.Teacher.User.LastName,
                        SessionClass = d.Class.Name,
                        SessionClassId = d.SessionClassId
                    }).ToArray(),
                    NoOfStudents = e.SessionClass.Select(s => s.Students).Count(),
                    NoOfClasses = e.SessionClass.Count(),
                    NoOfSubjects = e.SessionClass.Select(s => s.SessionClassSubjects).Count(),
                }
                ).FirstOrDefaultAsync();

            res.IsSuccessful = true;
            res.Result = result;
            return res;
        }


        async Task<APIResponse<bool>> ISessionService.ActivateTermAsync(Guid termId)
        {
            var res = new APIResponse<bool>();
            var term = await context.SessionTerm.FirstOrDefaultAsync(st => st.SessionTermId == termId);
            if (term == null)
            {
                res.Message.FriendlyMessage = "Session not found";
                return res;
            }
            term.IsActive = true;
            var otherTerms = await context.SessionTerm.Where(st => st.SessionId == term.SessionId && st.SessionTermId != term.SessionTermId).ToListAsync();
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
            var result = await context.Session.Include(d => d.Terms).Where(d => d.Deleted == false && d.IsActive == true).Select(d => new ActiveSession
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
                var savedSession = context.Session.FirstOrDefault(d => d.SessionId == Guid.Parse(req.SessionId));

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
    }
}
