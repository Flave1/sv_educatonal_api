using BLL;
using Contracts.Session;
using SMP.Contracts.Session;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SMP.BLL.Services.SessionServices
{
    public interface ITermService
    {
        Task<APIResponse<bool>> ActivateTermAsync(Guid termId);
        Task CreateSessionTermsAsync(Guid sessionId, int noOfTerms);
        SessionTermDto GetCurrentTerm();
        Task<APIResponse<List<Terms>>> GetSessionTermsAsync(Guid sessionId);
        SessionTermDto SelectTerm(Guid termId);
        List<SessionTermDto> GetTermsBySession(Guid sessionId);
        Task SetFirstTermActiveAsync(Guid sessionId);
    }
}