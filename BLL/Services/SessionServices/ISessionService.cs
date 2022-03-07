using Contracts.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.SessionServices
{
    public interface ISessionService
    {
        Task CreateSessionAsync(CreateUpdateSession session);
        Task ModifySessionAsync(CreateUpdateSession session);
        Task DeleteSessionAsync(Guid sessionId);
        Task<List<GetSession>> GetSessionsAsync();
        Task SwitchSessionAsync(string sessionId, bool switchValue);
    }
}
