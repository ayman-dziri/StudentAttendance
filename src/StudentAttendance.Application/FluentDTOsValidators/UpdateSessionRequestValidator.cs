using FluentValidation;
using StudentAttendance.src.StudentAttendance.Application.DTOs.Session.Requests;

namespace StudentAttendance.src.StudentAttendance.Application.FluentDTOsValidators
{
    public class UpdateSessionRequestValidator : AbstractValidator<UpdateSessionRequest>
    {
        public UpdateSessionRequestValidator()
        {
            RuleFor(x => x.Group)
               .NotEmpty()
               .WithMessage("Group cannot be empty");

            RuleFor(x => x.TeacherId)
                .NotEmpty()
                .When(x => x.TeacherId != null);

            RuleFor(x => x.EndTime)
                .GreaterThan(x => x.StartTime)
                .When(x => x.StartTime != default && x.EndTime != default);

            RuleFor(x => x.StartTime)
                .NotEmpty()
                .WithMessage("StartTime cannot be empty");


            RuleFor(x => x.IsValidated)
            .NotEmpty();
        }
    }
}
