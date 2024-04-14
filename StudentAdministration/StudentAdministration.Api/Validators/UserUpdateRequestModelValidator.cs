using FluentValidation;
using StudentAdministration.Communication.Users.Models;

namespace StudentAdministration.Api.Validators
{
    public class UserUpdateRequestModelValidator : AbstractValidator<UserUpdateRequestModel>
    {
        public UserUpdateRequestModelValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Id is required");

            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required");

            RuleFor(x => x.PartitionKey)
                .NotEmpty().WithMessage("Partition key is required");
        }
    }
}
