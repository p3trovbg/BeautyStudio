using AppointmentSystem.Features.Appointments.DTOs;
using FluentValidation;

namespace AppointmentSystem.Features.Appointments.Validators;

public class CreateAppointmentValidator : AbstractValidator<CreateAppointmentDto>
{
    public CreateAppointmentValidator()
    {
        RuleFor(x => x.OwnerId).NotEmpty();
        RuleFor(x => x.CustomerId).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.StartTime).NotEmpty();
        RuleFor(x => x.EndTime).NotEmpty().GreaterThan(x => x.StartTime).WithMessage("End time must be after start time.");
        RuleFor(x => x.Notes).MaximumLength(1000);
    }
}

public class UpdateAppointmentValidator : AbstractValidator<UpdateAppointmentDto>
{
    public UpdateAppointmentValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.StartTime).NotEmpty();
        RuleFor(x => x.EndTime).NotEmpty().GreaterThan(x => x.StartTime).WithMessage("End time must be after start time.");
        RuleFor(x => x.Status).IsInEnum();
        RuleFor(x => x.Notes).MaximumLength(1000);
    }
}
