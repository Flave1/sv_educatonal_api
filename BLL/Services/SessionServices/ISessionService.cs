using BLL.Filter;
using BLL.Wrappers;
using Contracts.Session;
using DAL.SessionEntities;
using SMP.DAL.Models.SessionEntities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.SessionServices
{
    public interface ISessionService
    {
        Task<APIResponse<CreateUpdateSession>> CreateSessionAsync(CreateUpdateSession session);
        Task<APIResponse<Session>> DeleteSessionAsync(Guid sessionId);
        Task<APIResponse<PagedResponse<List<GetSession>>>> GetSessionsAsync(PaginationFilter filter);
        Task<APIResponse<Session>> SwitchSessionAsync(string sessionId);
        Task<APIResponse<bool>> ActivateTermAsync(Guid termId);
        Task<APIResponse<ActiveSession>> GetActiveSessionsAsync();
        Task<APIResponse<GetSession>> GetSingleSessionAsync(string sessionId);
        Task<APIResponse<bool>> UpdateSessionHeadTeacherAsync(UpdateHeadTeacher req);
        Task<APIResponse<List<Terms>>> GetSessionTermsAsync(Guid sessionId);
        Task<APIResponse<List<GetSessionClass>>> GetSessionClassesAsync(Guid sessionId);
        SessionTerm GetPreviousSessionLastTermAsync(Guid sessionId);
    }
}
