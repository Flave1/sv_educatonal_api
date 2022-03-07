using Contracts.Session;
using DAL;
using DAL.SessionEntities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        async Task ISessionService.SwitchSessionAsync(string sessionId, bool switchValue)
        {
            var savedSession = context.Session.FirstOrDefault(d => d.SessionId == Guid.Parse(sessionId));
            if (savedSession == null)
                throw new ArgumentException("Session Not Found");

            if(switchValue && context.Session.Any(e => e.IsActive && e.SessionId != Guid.Parse(sessionId)))
                throw new ArgumentException("Running Session Detected");
             
            savedSession.IsActive = switchValue; 
            await context.SaveChangesAsync();
        }
        async Task ISessionService.CreateSessionAsync(CreateUpdateSession session)
        {
            context.Session.Add(new Session { StartDate = session.StartDate, EndDate = session.EndDate });
            await context.SaveChangesAsync();
        }

        async Task ISessionService.ModifySessionAsync(CreateUpdateSession session)
        {
            var savedSession = context.Session.FirstOrDefault(d => d.SessionId == Guid.Parse(session.SessionId));
            if(savedSession == null) 
                throw new ArgumentException("Session Not Found");
            savedSession.StartDate = session.StartDate;
            savedSession.EndDate = session.EndDate;
            await context.SaveChangesAsync();
        }

        async Task ISessionService.DeleteSessionAsync(Guid sessionId)
        {
            var savedSession = context.Session.FirstOrDefault(d => d.SessionId == sessionId);
            if (savedSession == null)
                throw new ArgumentException("Session Not Found");
            savedSession.Deleted = true; 
            await context.SaveChangesAsync();
        }

        async Task<List<GetSession>> ISessionService.GetSessionsAsync()
        {
            return await context.Session.Where(d => d.Deleted == false).Select(e => 
                new GetSession { 
                    EndDate = e.EndDate, 
                    SessionId = e.SessionId.ToString(), 
                    StartDate = e.StartDate,
                    IsActive = e.IsActive
                }
                ).ToListAsync();
        }
    }
}
