using Contracts.Class;
using DAL;
using DAL.ClassEntities;
using Microsoft.EntityFrameworkCore; 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks; 
namespace BLL.ClassServices
{
    public class ClassService : IClassService
    {
        private readonly DataContext context;

        public ClassService(DataContext context)
        {
            this.context = context;
        }
         
        async Task IClassService.CreateSessionClassAsync(SessionClassCommand sClass)
        {
            if(context.SessionClass.Any(ss => ss.InSession == true && ss.ClassId == Guid.Parse(sClass.ClassId))) 
                throw new ArgumentException($"Same Class In Session Detected");

            context.SessionClass.Add(new SessionClass 
            {
                ClassId = Guid.Parse(sClass.ClassId),
                FormTeacherId = Guid.Parse(sClass.FormTeacherId),
                ClassCaptainId = Guid.Parse(sClass.ClassCaptainId),
                //SessionClassId = Guid.Parse(sClass.SessionClassId),
                SessionId = Guid.Parse(sClass.SessionId),
                InSession = sClass.InSession,  
            });
            await context.SaveChangesAsync(); 
        }

        async Task<List<GetSessionClass>> IClassService.GetSessionClassesAsync()
        {
            return await context.SessionClass.Where(r => r.InSession)
                .Include(rr => rr.Class)
                .Include(rr=> rr.Session)
                //.Include(rr => rr.Students)
                .Include(rr => rr.Teacher).ThenInclude(uuu => uuu.User).Select(g => new GetSessionClass(g)).ToListAsync();
        }

        async Task<List<GetSessionClass>> IClassService.GetSessionClassesBySessionAsync(DateTime? startDate, DateTime? endDate)
        {
            var query = context.SessionClass
         .Include(rr => rr.Class)
         .Include(rr => rr.Session)
         .Include(rr => rr.Students).ThenInclude(uuu => uuu.User)
         .Include(rr => rr.Teacher).ThenInclude(uuu => uuu.User).Where(r => r.InSession);
            if (startDate != null && startDate.HasValue)
                query = query.Where(v => v.Session.StartDate.Date == startDate.Value.Date);

            if (endDate != null && endDate.HasValue)
                query = query.Where(v => v.Session.StartDate.Date == endDate.Value.Date);

            return await query.Select(g => new GetSessionClass(g)).ToListAsync();
        }
    }
}
