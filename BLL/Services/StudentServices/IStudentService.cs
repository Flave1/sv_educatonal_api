using BLL.Filter;
using BLL.Wrappers;
using Contracts.Common;
using Contracts.Options;
using DAL.StudentInformation;
using SMP.Contracts.Students;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BLL.StudentServices
{
    public interface IStudentService
    {
        Task<APIResponse<PagedResponse<List<GetStudentContacts>>>> GetAllStudensAsync(PaginationFilter filter);
        Task<APIResponse<StudentContact>> CreateStudenAsync(StudentContactCommand student);
        Task<APIResponse<StudentContact>> UpdateStudenAsync(StudentContactCommand student);
        Task<APIResponse<GetStudentContacts>> GetSingleStudentAsync(Guid studentContactId);
        Task<APIResponse<bool>> DeleteStudentAsync(MultipleDelete request);
        Task ChangeClassAsync(Guid studentId, Guid classId);
        Task<APIResponse<UpdateProfileByStudentRequest>> UpdateProfileByStudentAsync(UpdateProfileByStudentRequest student);
        Task<APIResponse<StudentContact>> UploadStudentsAsync();
        //Task<APIResponse<StudentContact>> UploadStudentsAsync(StudentContactCommand student);
        Task<APIResponse<GetStudentContactCbt>> GetSingleStudentByRegNoCbtAsync(string studentRegNo, string clientId);
        Task<APIResponse<PagedResponse<List<GetStudentContactCbt>>>> GetStudentBySessionClassCbtAsync(PaginationFilter filter, string sessionClassId, string clientId);
        Task<APIResponse<List<GetStudentContactCbt>>> GetAllStudentBySessionClassCbtAsync(string sessionClassId, string clientId);
        Task<APIResponse<byte[]>> DownloadStudentTemplate();
        IQueryable<StudentContact> GetStudent(Guid studentContactId);
        //StudentContact GetStudentByUserId(string userId);
    }
}
