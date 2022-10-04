using BLL;
using BLL.Filter;
using BLL.Wrappers;
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
        Task<APIResponse<PagedResponse<List<EnrolledStudents>>>> GetEnrolledStudentsAsync(Guid sessionClassId, PaginationFilter filter);
        Task<APIResponse<PagedResponse<List<EnrolledStudents>>>> GetUnenrrolledStudentsAsync(PaginationFilter filter);
        void UnenrollStudent(Guid studentId);
    }
}