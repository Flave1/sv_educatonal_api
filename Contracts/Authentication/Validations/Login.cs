using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Contracts.Authentication.Validations
{
   public class LoginValHandler : AbstractValidator<LoginCommand>
    {
        public LoginValHandler()
        {
            RuleFor(e => e.UserName).NotEmpty().WithMessage("User name is required");
            RuleFor(e => e.Password).NotEmpty().Must(IsPasswordCharactersValid).WithMessage("New password must contain atleast 8 characters, uppercase, lowercase, symbols and alphanumeric ");  
        }
        public static bool IsPasswordCharactersValid(string password)
        {
            var hasNumber = new Regex(@"[0-9]+");
            var hasUpperChar = new Regex(@"[A-Z]+");
            var hasMinimum8Chars = new Regex(@".{8,}");

            return  hasNumber.IsMatch(password) && hasUpperChar.IsMatch(password) && hasMinimum8Chars.IsMatch(password);
        }
    }

  
}
