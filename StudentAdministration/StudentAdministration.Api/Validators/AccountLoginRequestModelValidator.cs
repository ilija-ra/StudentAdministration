using FluentValidation;
using StudentAdministration.Communication.Accounts.Models;

namespace StudentAdministration.Api.Validators
{
    public class AccountLoginRequestModelValidator : AbstractValidator<AccountLoginRequestModel>
    {
        public AccountLoginRequestModelValidator()
        {
            RuleFor(x => x.EmailAddress)
                .NotEmpty().WithMessage("Email address is required")
                .EmailAddress().WithMessage("Invalid email address format");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required");
        }
    }
}
