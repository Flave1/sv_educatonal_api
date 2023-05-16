using DAL;
using Microsoft.AspNetCore.Http;
using SMP.DAL.Models.Authentication;
using System;
using System.Linq;

namespace SMP.BLL.Services.AuthenticationServices
{
    public class OtpService : IOtpService
    {
        private readonly DataContext context;
        private readonly IHttpContextAccessor accessor;

        public OtpService(DataContext context, IHttpContextAccessor accessor)
        {
            this.context = context;
            this.accessor = accessor;
        }

        bool IOtpService.IsOtpValid(string otp, string client)
        {
            accessor.HttpContext.Items["smsClientId"] = client;
            var otpToken = context.OTP.FirstOrDefault(d => d.Token == otp && d.ClientId == client);
            if(otpToken == null)
                return false;
            context.OTP.Remove(otpToken);
            context.SaveChanges();
            return true;
        }

        OTP IOtpService.GenerateOtp(string client)
        {
            accessor.HttpContext.Items["smsClientId"] = client;
            var otp = new OTP
            {
                Id = Guid.NewGuid().ToString(),
                ClientId = client,
                Token = GenerateOTP(5),
                ExpireAt = DateTime.Now.AddMinutes(30),
            };
            context.OTP.Add(otp);
            context.SaveChanges();
            return otp;
        }

        static string GenerateOTP(int length)
        {
            Random random = new Random();
            string otp = "";
            for (int i = 0; i < length; i++)
            {
                otp += random.Next(0, 9).ToString();
            }
            return otp;
        }
    }
}
