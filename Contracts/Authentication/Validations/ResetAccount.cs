using Contracts.Options;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Authentication.Validations
{
    public class ResetAccountValHAndler: AbstractValidator<ResetPassword>
    {
        //public ResetAccountValHAndler()
        //{
        //    RuleFor(er => er.ResetOption).NotEmpty().WithMessage("Please check all field");
        //    RuleFor(er => er.ResetOptionValue).NotEmpty().WithMessage("Please check all field");
        //    RuleFor(er => er.UserType).NotEmpty().WithMessage("Please check all field");
        //}

     
    }
}
