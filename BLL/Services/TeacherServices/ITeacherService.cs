using BLL;
using BLL.Filter;
using BLL.Wrappers;
using Contracts.Authentication;
using Contracts.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SMP.BLL.Services.TeacherServices
{
    public interface ITeacherService
    {
        Task<APIResponse<bool>> DeleteTeacherAsync(MultipleDelete req);
        Task<APIResponse<PagedResponse<List<ApplicationUser>>>> GetAllTeachersAsync(PaginationFilter filter);
        Task<APIResponse<UserCommand>> UpdateTeacherAsync(UserCommand userDetail);
        Task<APIResponse<UserCommand>> CreateTeacherAsync(UserCommand request);
        Task<APIResponse<ApplicationUser>> GetSingleTeacherAsync(Guid teacherId);
        Task<APIResponse<List<ApplicationUser>>> GetAllActiveTeachersAsync();
        Task<APIResponse<UpdateProfileByTeacher>> UpdateTeacherProfileByTeacherAsync(UpdateProfileByTeacher userDetail);
        Task<APIResponse<TeacheerClassAndSibjects>> GetSingleTeacherClassesAndSubjectsAsync(Guid teacherId);
    }
}