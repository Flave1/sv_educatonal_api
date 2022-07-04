using Contracts.Common;
using Contracts.Options;
using DAL.StudentInformation;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.StudentServices
{
    public interface IStudentService
    {
        Task<APIResponse<List<GetStudentContacts>>> GetAllStudensAsync();
        Task<APIResponse<StudentContact>> CreateStudenAsync(StudentContactCommand student);
        Task<APIResponse<StudentContact>> UpdateStudenAsync(StudentContactCommand student);
        Task<APIResponse<GetStudentContacts>> GetSingleStudentAsync(Guid studentContactId);
        Task<APIResponse<bool>> DeleteStudentAsync(MultipleDelete request);
        Task ChangeClassAsync(Guid studentId, Guid classId);
    }
}
