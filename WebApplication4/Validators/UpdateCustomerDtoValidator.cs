using FluentValidation;
using WebApplication4.DTOs;

namespace WebApplication4.Validators;

public class UpdateCustomerDtoValidator : AbstractValidator<UpdateCustomerDto>
{
    public UpdateCustomerDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Address)
            .MaximumLength(250)
            .When(x => x.Address != null);

        RuleFor(x => x.PhoneNumber)
            .MaximumLength(50)
            .When(x => x.PhoneNumber != null);
    }
}