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
        Task CreateSessionClassAsync(SessionClassCommand sClass);
        Task<List<GetSessionClass>> GetSessionClassesAsync();
        Task<List<GetSessionClass>> GetSessionClassesBySessionAsync(DateTime? StartDate, DateTime? EndDate); 
    }
}
