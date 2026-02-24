using AppointmentSystem.Features.Owners.DTOs;
using FluentValidation;

namespace AppointmentSystem.Features.Owners.Validators;

public class CreateOwnerValidator : AbstractValidator<CreateOwnerDto>
{
    public CreateOwnerValidator()
    {
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(254);
        RuleFor(x => x.PhoneNumber).MaximumLength(20);
    }
}

public class UpdateOwnerValidator : AbstractValidator<UpdateOwnerDto>
{
    public UpdateOwnerValidator()
    {
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(254);
        RuleFor(x => x.PhoneNumber).MaximumLength(20);
    }
}
