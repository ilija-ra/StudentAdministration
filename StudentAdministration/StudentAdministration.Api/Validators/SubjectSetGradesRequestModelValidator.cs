using FluentValidation;
using StudentAdministration.Communication.Subjects.Models;

namespace StudentAdministration.Api.Validators
{
    public class SubjectSetGradesRequestModelValidator : AbstractValidator<SubjectSetGradesRequestModel>
    {
        public SubjectSetGradesRequestModelValidator()
        {
            RuleFor(x => x.SubjectId)
                .NotEmpty().WithMessage("Subject id is required");

            RuleFor(x => x.SubjectPartitionKey)
                .NotEmpty().WithMessage("Subject partition key is required");

            RuleFor(x => x.StudentId)
                .NotEmpty().WithMessage("Student id is required");

            RuleFor(x => x.StudentPartitionKey)
                .NotEmpty().WithMessage("Student partition key is required");

            RuleFor(x => x.Grade)
                .NotNull().WithMessage("Grade is required")
                .InclusiveBetween(5, 10).WithMessage("Grade must be between 5 and 10");
        }
    }
}
