using FluentValidation;
using WebApplication4.DTOs;

namespace WebApplication4.Validators;

public class UpdateInvoiceDtoValidator : AbstractValidator<UpdateInvoiceDto>
{
    public UpdateInvoiceDtoValidator()
    {
        RuleFor(x => x.StartDate).NotEmpty();
        RuleFor(x => x.EndDate)
            .GreaterThan(x => x.StartDate)
            .WithMessage("EndDate must be after StartDate");

        RuleForEach(x => x.Rows)
            .SetValidator(new UpdateInvoiceRowDtoValidator()); 

        RuleFor(x => x.Comment)
            .MaximumLength(500)
            .When(x => x.Comment != null);
    }
}
public class UpdateInvoiceRowDtoValidator : AbstractValidator<UpdateInvoiceRowDto>
{
    public UpdateInvoiceRowDtoValidator()
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