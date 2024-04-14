using FluentValidation;
using StudentAdministration.Communication.Subjects.Models;

namespace StudentAdministration.Api.Validators
{
    public class SubjectDropOutRequestModelValidator : AbstractValidator<SubjectDropOutRequestModel>
    {
        public SubjectDropOutRequestModelValidator()
        {
            RuleFor(x => x.SubjectId)
                .NotEmpty().WithMessage("Subject Id is required");

            RuleFor(x => x.StudentId)
                .NotEmpty().WithMessage("Student Id is required");
        }
    }
}
