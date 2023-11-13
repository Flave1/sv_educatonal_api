namespace SMP.Contracts.Authentication
{
    public class ForgotPassword
    {
        public string Email { get; set; }
        public string SchoolUrl { get; set; }
        public int UserType { get; set; }
    }
}
