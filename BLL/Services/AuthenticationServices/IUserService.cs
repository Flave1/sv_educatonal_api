using APIResponses;
using Contracts.Authentication;
using Contracts.Options;
using DAL.Authentication;
using DAL.StudentInformation;
using DAL.TeachersInfor;
using Microsoft.AspNetCore.Http;
using SMP.Contracts.Authentication;
using SMP.DAL.Models.Parents;
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
        //Task UpdateStudentUserProfileImageAsync(IFormFile file, string studentId);
        //Task UpdateTeacherUserProfileImageAsync(IFormFile file, AppUser account);
        Task<APIResponse<LoginSuccessResponse>> ChangePasswordAsync(ChangePassword request);
        Task<APIResponse<SmpStudentValidationResponse>> ValidateUserInformationFromMobileAsync(UserInformationFromMobileRequest request);
        Task<string> CreateStudentUserAccountAsync(UploadStudentExcel student, string regNo, string regNoFormat);
        Task UpdateStudentUserAccountAsync(UploadStudentExcel student, string userAccountId, string studentId);
        Task<string> CreateParentUserAccountAsync(string email, string phone);
        Task UpdateParentUserAccountAsync(string email, string phone, string id, Guid parentId);
        Task<APIResponse<bool>> ForgotPassword(ForgotPassword request);
        Task<APIResponse<bool>> ResetPassword(ResetAccount request);
        Task<bool> TeacherAccountByEmailExist(string email);
        Task<bool> TeacherAccountByEmailExistOnUpdate(string email, Guid teacherId);
        Task<APIResponse<AuthenticationResult>> ValidateEmailAsync(ValidateEmail request);
        Task<APIResponse<AuthenticationResult>> ValidateOTPAsync(ValidateOtp request);
        Task<APIResponse<bool>> ResetPasswordMobile(ResetAccountMobile request);
        Task<FwsAPIResponse<string>> CreateUserOnFws(CreateUserCommand request);
        Task<FwsAPIResponse<string>> UpdateUserOnFws(UpdateUserCommand request);
    }
}
