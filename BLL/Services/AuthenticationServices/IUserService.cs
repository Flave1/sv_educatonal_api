using Contracts.Authentication;
using Contracts.Options;
using DAL.Authentication;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.AuthenticationServices
{
    public interface IUserService
    {
        Task CreateTeacherAsync(string email); 
        Task UpdateTeacherAsync(UpdateTeacher userDetail);
        Task<List<ApplicationUser>> GetAllTeachersAsync(); 
        Task DeleteUserAsync(string UserId);
        Task AddUserToRoleAsync(string roleId, AppUser user = null, string[] userId = null);
        Task<string> CreateStudentUserAccountAsync(StudentContactCommand user, string regNo, string regNoFormat);
        Task GenerateResetLinkAndSendToUserEmail(ResetPassword request);
        void ValidateResetOption(ResetPassword request);
        Task<AuthenticationResult> ResetAccountAsync(ResetAccount request);
    }
}
