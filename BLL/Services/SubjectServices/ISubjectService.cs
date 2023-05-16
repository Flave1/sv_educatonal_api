using Contracts.Class;
using Contracts.Common;
using DAL.SubjectModels;
using SMP.Contracts.ClassModels;
using SMP.Contracts.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.Services.SubjectServices
{
    public interface ISubjectService
    {
        Task<APIResponse<Subject>> CreateSubjectAsync(ApplicationLookupCommand subject);
        Task<APIResponse<Subject>> UpdateSubjectAsync(string Name, string Id, bool isActive);
        Task<APIResponse<List<GetApplicationLookups>>> GetAllSubjectsAsync();
        Task<APIResponse<Subject>> DeleteSubjectAsync(MultipleDelete request);
        Task<APIResponse<List<GetApplicationLookups>>> GetAllActiveSubjectsAsync();
        APIResponse<List<DropdownSelect>> GetAllStudentSubjects();
        APIResponse<Guid> GetSubjectTeacher(Guid subjectId);
        Task<APIResponse<List<SessionClassSubjects2>>> GetAllSubjects2Async(Guid sessionClassId);
    }
}