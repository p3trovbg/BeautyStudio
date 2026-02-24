using AppointmentSystem.Features.Customers.DTOs;
using FluentValidation;

namespace AppointmentSystem.Features.Customers.Validators;

public class CreateCustomerValidator : AbstractValidator<CreateCustomerDto>
{
    public CreateCustomerValidator()
    {
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(254);
        RuleFor(x => x.PhoneNumber).MaximumLength(20);
    }
}

public class UpdateCustomerValidator : AbstractValidator<UpdateCustomerDto>
{
    public UpdateCustomerValidator()
    {
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(254);
        RuleFor(x => x.PhoneNumber).MaximumLength(20);
    }
}
