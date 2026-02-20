using FluentValidation;
using StudentAttendance.src.StudentAttendance.Application.DTOs.Session.Requests;
using System.Data;

namespace StudentAttendance.src.StudentAttendance.Application.FluentDTOsValidators;

public class CreateSessionRequestValidator : AbstractValidator<CreateSessionRequest>
{
    public CreateSessionRequestValidator()
    {

        RuleFor(x => x.StartTime)
                .NotEmpty()
                .WithMessage("StartTime cannot be empty");

        RuleFor(x => x.EndTime)
            .GreaterThan(x => x.StartTime)
            .WithMessage("EndTime must be after StartTime");

        RuleFor(x => x.TeacherId)
            .NotEmpty();

        RuleFor(x => x.Group)
            .NotEmpty();

        RuleFor(x => x.IsValidated)
           .NotEmpty();


    }
}






