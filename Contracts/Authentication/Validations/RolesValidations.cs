using FluentValidation;

namespace Contracts.Authentication.Validations
{
    public class RolesValidationhandler : AbstractValidator<ApplicationRoles>
    {
        public RolesValidationhandler()
        {
            RuleFor(d => d.Name).NotEmpty().WithMessage("Role name is required");
        }
    }

    public class AddToRoleValidationhandler : AbstractValidator<AddToRole>
    {
        public AddToRoleValidationhandler()
        {
            RuleFor(d => d.RoleId).NotEmpty().WithMessage("Role name is required");
            RuleFor(d => d.UserIds).NotEmpty().WithMessage("User is required");
        }
    }
}
