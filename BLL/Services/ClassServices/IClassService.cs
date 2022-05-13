using Contracts.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.ClassServices
{
    public interface IClassService
    {
        Task<APIResponse<SessionClassCommand>> CreateSessionClassAsync(SessionClassCommand sClass);
        Task<APIResponse<List<GetSessionClass>>> GetSessionClassesAsync();
        Task<APIResponse<List<GetSessionClass>>> GetSessionClassesBySessionAsync(DateTime? StartDate, DateTime? EndDate); 
    }
}
