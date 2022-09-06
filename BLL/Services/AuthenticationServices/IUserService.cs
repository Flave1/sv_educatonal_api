using Contracts.Authentication;
using Contracts.Options;
using DAL.Authentication;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.AuthenticationServices
{
    public interface IUserService
    {
        Task<APIResponse<string[]>> AddUserToRoleAsync(string roleId, AppUser user, string[] userIds);
        void ValidateResetOption(ResetPassword request);
        Task GenerateResetLinkAndSendToUserEmail(ResetPassword request);
        Task<APIResponse<AuthenticationResult>> ResetAccountAsync(ResetAccount request);
        Task<string> CreateStudentUserAccountAsync(StudentContactCommand student, string regNo, string regNoFormat);
        Task UpdateStudentUserAccountAsync(StudentContactCommand student);
        Task UpdateStudentUserProfileImageAsync(IFormFile file, string studentId);
        Task UpdateTeacherUserProfileImageAsync(IFormFile file, AppUser account);
        Task<APIResponse<LoginSuccessResponse>> ChangePasswordAsync(ChangePassword request);
    }
}
