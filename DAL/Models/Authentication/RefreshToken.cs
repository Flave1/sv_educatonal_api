using System;
using System.ComponentModel.DataAnnotations;

namespace DAL.Authentication
{
    public class RefreshToken
    {
        [Key]
        public string JwtId { get; set; }
        public string Token { get; set; }
        public string UserId { get; set; }
        public bool Invalidated { get; set; }
        public bool Used { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
} 