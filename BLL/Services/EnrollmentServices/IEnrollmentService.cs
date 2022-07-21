using BLL;
using DAL.StudentInformation;
using SMP.Contracts.Enrollment;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SMP.BLL.Services.EnrollmentServices
{
    public interface IEnrollmentService
    {
        Task<APIResponse<Enroll>> EnrollStudentsAsyncAsync(Enroll req);
        Task<APIResponse<UnEnroll>> UnenrollStudentsAsyncAsync(UnEnroll req);
        Task<APIResponse<List<EnrolledStudents>>> GetAllEnrrolledStudentsAsync(Guid sessionClassId);
        Task<APIResponse<List<EnrolledStudents>>> GetAllUnenrrolledStudentsAsync();
        void UnenrollStudent(Guid studentId);
    }
}