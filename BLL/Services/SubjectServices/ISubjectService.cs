using Contracts.Class;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.Services.SubjectServices
{
    public interface ISubjectService
    {
        Task CreateSubjectAsync(string subjectName);
        Task UpdateSubjectAsync(string Name, string Id, bool isActive);
        Task<List<GetApplicationLookups>> GetAllSubjectsAsync();
        Task DeleteSubjectAsync(string Id);
    }
}