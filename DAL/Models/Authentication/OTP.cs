using DAL;
using System;
using System.ComponentModel.DataAnnotations;

namespace SMP.DAL.Models.Authentication
{
    public class OTP: CommonEntity
    {
        [Key]
        public string Id { get; set; }
        public string Token { get; set; }
        public DateTime ExpireAt { get; set;}

    }
}
