using FluentValidation;
using WebApplication4.DTOs;

namespace WebApplication4.Validators;

public class CreateInvoiceRowDtoValidator : AbstractValidator<CreateInvoiceRowDto>
{
    public CreateInvoiceRowDtoValidator()
    {
        RuleFor(x => x.Service)
            .NotEmpty().WithMessage("Service name cannot be empty")
            .MaximumLength(200).WithMessage("Service name cannot exceed 200 characters");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than zero");
                
        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Amount must be greater than zero");
    }
}