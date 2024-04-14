using FluentValidation;
using StudentAdministration.Communication.Accounts.Models;

namespace StudentAdministration.Api.Validators
{
    public class AccountRegisterRequestModelValidator : AbstractValidator<AccountRegisterRequestModel>
    {
        public AccountRegisterRequestModelValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required");

            RuleFor(x => x.Index)
                .NotEmpty().WithMessage("Index is required");

            RuleFor(x => x.EmailAddress)
                .NotEmpty().WithMessage("Email address is required")
                .EmailAddress().WithMessage("Invalid email address format");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required");
        }
    }
}
