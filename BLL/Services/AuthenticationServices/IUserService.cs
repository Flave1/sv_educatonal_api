﻿using Contracts.Authentication;
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
        Task UpdateStudentUserAccountAsync(UploadStudentExcel student, string userAccountId);
        Task<string> CreateParentUserAccountAsync(string email, string phone);
        Task UpdateParentUserAccountAsync(string email, string phone, string id);
        Task<APIResponse<bool>> ForgotPassword(ForgotPassword request);
        Task<APIResponse<bool>> ResetPassword(ResetAccount request);
      
    }
}
