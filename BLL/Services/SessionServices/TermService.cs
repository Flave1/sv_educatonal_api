using BLL.SessionServices;
using BLL;
using DAL;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SMP.BLL.Utilities;
using SMP.Contracts.Session;
using SMP.DAL.Models.SessionEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.ClassEntities;
using Contracts.Session;

namespace SMP.BLL.Services.SessionServices
{
    public class TermService : ITermService
    {
        private readonly DataContext context;
        private readonly string smsClientId;

        public TermService(DataContext context, IHttpContextAccessor accessor)
        {
            smsClientId = accessor.HttpContext.User.FindFirst(x => x.Type == "smsClientId")?.Value;
            this.context = context;
        }

        async Task ITermService.SetFirstTermActiveAsync(Guid sessionId)
        {
            var sessionTerms = context.SessionTerm.Where(tx => tx.SessionId == sessionId && tx.ClientId == smsClientId).ToList();
            if (sessionTerms.Any())
            {
                var firstTerm = sessionTerms.First();
                firstTerm.IsActive = true;
                await context.SaveChangesAsync();
            }
        }

        async Task ITermService.CreateSessionTermsAsync(Guid sessionId, int noOfTerms)
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

        SessionTermDto ITermService.GetCurrentTerm() =>
           context.SessionTerm.Where(d => d.ClientId == smsClientId && d.IsActive == true).Select(c => new SessionTermDto(c)).FirstOrDefault();


        SessionTermDto ITermService.SelectTerm(Guid termId) =>
            context.SessionTerm.Where(d => d.ClientId == smsClientId && termId == d.SessionTermId).Select(c => new SessionTermDto(c)).FirstOrDefault();

        List<SessionTermDto> ITermService.GetTermsBySession(Guid sessionId) =>
            context.SessionTerm.Where(d => d.ClientId == smsClientId && sessionId == d.SessionId).Select(c => new SessionTermDto(c)).ToList();

        async Task<APIResponse<bool>> ITermService.ActivateTermAsync(Guid termId)
        {
            var res = new APIResponse<bool>();

            var term = await context.SessionTerm.FirstOrDefaultAsync(st => st.SessionTermId == termId && st.ClientId == smsClientId);
            if (term == null)
            {
                res.Message.FriendlyMessage = "Session not found";
                return res;
            }
            term.IsActive = true;

            await DeactivateOtherTermsAsync(term);

            SetActiveClasses(term);

            await context.SaveChangesAsync();
            res.Result = true;
            res.IsSuccessful = true;
            res.Message.FriendlyMessage = $"Successfuly activated {term.TermName} term";
            return res;
        }

        List<SessionClass> ActiveClasses(Guid sessionId) =>
            context.SessionClass.Where(x => x.SessionId == sessionId && smsClientId == x.ClientId && x.Deleted == false).ToList();

        private async Task DeactivateOtherTermsAsync(SessionTerm term)
        {
            var otherTerms = await context.SessionTerm
                .Where(st => st.SessionId == term.SessionId && st.SessionTermId != term.SessionTermId && st.ClientId == smsClientId)
                .ToListAsync();

            if (otherTerms.Any())
                foreach (var otherTerm in otherTerms)
                    otherTerm.IsActive = false;
        }

        private  void SetActiveClasses(SessionTerm term)
        {
            var activeClasses = ActiveClasses(term.SessionId);
            for (int i = 0; i < activeClasses.Count; i++)
            {
                activeClasses[i].IsPromoted = false;
                activeClasses[i].IsPublished = false;
                activeClasses[i].SessionTermId = term.SessionTermId;
            }
        }

        async Task<APIResponse<List<Terms>>> ITermService.GetSessionTermsAsync(Guid sessionId)
        {
            var res = new APIResponse<List<Terms>>();
            var result = (this as ITermService).GetTermsBySession(sessionId).Select(t => new Terms
            {
                IsActive = t.IsActive,
                SessionTermId = t.SessionTermId,
                TermName = t.TermName,
            }).ToList();

            res.IsSuccessful = true;
            res.Result = result;
            return await Task.Run(() => res);
        }
    }
}
