using FluentValidation;
using StudentAdministration.Communication.Subjects.Models;

namespace StudentAdministration.Api.Validators
{
    public class SubjectEnrollRequestModelValidator : AbstractValidator<SubjectEnrollRequestModel>
    {
        public SubjectEnrollRequestModelValidator()
        {
            RuleFor(x => x.SubjectId)
                .NotEmpty().WithMessage("Subject id is required");

            RuleFor(x => x.SubjectPartitionKey)
                .NotEmpty().WithMessage("Subject partition key is required");

            RuleFor(x => x.StudentId)
                .NotEmpty().WithMessage("Student id is required");

            RuleFor(x => x.StudentPartitionKey)
                .NotEmpty().WithMessage("Student partition key is required");

            RuleFor(x => x.StudentIndex)
                .NotEmpty().WithMessage("Student index is required");

            RuleFor(x => x.StudentFullName)
                .NotEmpty().WithMessage("Student full name is required");

            RuleFor(x => x.ProfessorFullName)
                .NotEmpty().WithMessage("Professor full name is required");

            RuleFor(x => x.Grade)
                .NotNull().WithMessage("Grade is required")
                .GreaterThanOrEqualTo(0).WithMessage("Grade must be a positive integer");
        }
    }
}
