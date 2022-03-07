using Contracts.Options;
using FluentValidation;

namespace Contracts.Students.Validations
{
    public class StudentValHandler : AbstractValidator<StudentContactCommand>
    {
        public StudentValHandler()
        {
            RuleFor(d => d.FirstName).NotEmpty().MinimumLength(2);
            RuleFor(d => d.LastName).NotEmpty().MinimumLength(2);
            RuleFor(d => d.MiddleName).NotEmpty().MinimumLength(2); 
        }
    }
}
 