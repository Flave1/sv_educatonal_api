using BLL;
using DAL.StudentInformation;
using SMP.Contracts.Enrollment;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SMP.BLL.Services.EnrollmentServices
{
    public interface IEnrollmentService
    {
        Task<APIResponse<Enroll>> EnrollStudentsAsyncAsync(Enroll req);
        Task<APIResponse<UnEnroll>> UnenrollStudentsAsyncAsync(UnEnroll req);
        Task<APIResponse<List<EnrolledStudents>>> GetAllEnrrolledStudentsAsync();
        Task<APIResponse<List<EnrolledStudents>>> GetAllUnenrrolledStudentsAsync(); 
    }
}