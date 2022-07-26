﻿using Contracts.Authentication;
using Contracts.Options;
using DAL.Authentication;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.AuthenticationServices
{
    public interface IUserService
    {
        Task AddUserToRoleAsync(string roleId, AppUser user, string[] userIds);
        void ValidateResetOption(ResetPassword request);
        Task GenerateResetLinkAndSendToUserEmail(ResetPassword request);
        Task<APIResponse<AuthenticationResult>> ResetAccountAsync(ResetAccount request);
        Task<string> CreateStudentUserAccountAsync(StudentContactCommand student, string regNo, string regNoFormat);
        Task UpdateStudentUserAccountAsync(StudentContactCommand student);
    }
}
