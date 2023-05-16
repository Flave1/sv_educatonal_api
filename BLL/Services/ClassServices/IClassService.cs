using Contracts.Class;
using Contracts.Options;
using DAL.ClassEntities;
using SMP.Contracts.ClassModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.ClassServices
{
    public interface IClassService
    {
        //Task<APIResponse<SessionClassCommand>> CreateSessionClassAsync(SessionClassCommand sClass);
        Task<APIResponse<List<GetSessionClass>>> GetSessionClassesAsync(string sessionId);
        Task<APIResponse<List<GetSessionClass>>> GetSessionClassesBySessionAsync(string StartDate, string EndDate);
        //Task<APIResponse<SessionClassCommand>> UpdateSessionClassAsync(SessionClassCommand sClass);
        Task<APIResponse<GetSessionClass>> GetSingleSessionClassesAsync(Guid sessionClassId);
        Task<APIResponse<bool>> DeleteSessionClassesAsync(Guid sessionClassId);
        Task<APIResponse<List<GetStudentContacts>>> GetClassStudentsClassesAsync(Guid sessionClassId);
        Task<APIResponse<List<GetSessionClass>>> GetSessionClasses2Async();
        Task<APIResponse<List<GetSessionClass>>> GetSessionClasses1Async(string sessionId);
        Task<APIResponse<List<GetSessionClassCbt>>> GetSessionClassesCbtAsync(string clientId);
        Task<APIResponse<GetSessionClassCbt>> GetSessionClassesCbtByRegNoAsync(string registrationNo, string clientId);
        Task<APIResponse<SessionClassCommand>> CreateSessionClassSubjectsAsync(ClassSubjectcommand request);
        Task<APIResponse<SessionClassCommand2>> CreateSessionClass2Async(SessionClassCommand2 sClass);
        Task<APIResponse<SessionClassCommand>> UpdateSessionClass2Async(SessionClassCommand2 request);
        Task<APIResponse<GetSessionClass>> GetSingleSessionClassesWithoutSubjectsAndStudentsAsync(Guid sessionClassId);
        Task<APIResponse<List<ClassSubjects>>> GetSessionClassSubjects(Guid sessionClassId);
        IQueryable<SessionClass> GetSessionClass(Guid sessionClassId);
        ClassDto GetSessionClassByLkp(Guid classLkp);
        Task<APIResponse<List<GetStudentContacts2>>> GetClassStudentsClassesMobileAsync(Guid sessionClassId);
    }
}
