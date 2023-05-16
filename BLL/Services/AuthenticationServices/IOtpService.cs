using SMP.DAL.Models.Authentication;

namespace SMP.BLL.Services.AuthenticationServices
{
    public interface IOtpService
    {
        OTP GenerateOtp(string client);
        bool IsOtpValid(string otp, string client);
    }
}