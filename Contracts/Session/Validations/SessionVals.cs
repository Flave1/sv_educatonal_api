using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Session.Validations
{
    public class SessionValsHandler : AbstractValidator<CreateUpdateSession>
    {
        public SessionValsHandler()
        {
            RuleFor(d => d.EndDate).NotEmpty();
            RuleFor(d => d.StartDate).NotEmpty();
        }
    }
}
