using FluentValidation;
using WebApplication4.DTOs;

namespace WebApplication4.Validators
{

    public class CreateInvoiceDtoValidator : AbstractValidator<CreateInvoiceDto>
    {
        public CreateInvoiceDtoValidator()
        {
        
            RuleFor(x => x.CustomerId)
                .NotEmpty().WithMessage("CustomerId cannot be empty")
                .Must(id => Guid.TryParse(id.ToString(), out _))
                .WithMessage("CustomerId must be a valid GUID");

           
            RuleFor(x => x.StartDate)
                .NotEmpty().WithMessage("StartDate is required")
                .LessThanOrEqualTo(DateTimeOffset.UtcNow.AddYears(1))
                .WithMessage("StartDate cannot be more than 1 year in the future");

         
            RuleFor(x => x.EndDate)
                .NotEmpty().WithMessage("EndDate is required")
                .GreaterThanOrEqualTo(x => x.StartDate)
                .WithMessage("EndDate must be after StartDate")
                .LessThanOrEqualTo(DateTimeOffset.UtcNow.AddYears(2))
                .WithMessage("EndDate cannot be more than 2 years in the future");

      
            RuleFor(x => x.Rows)
                .NotEmpty().WithMessage("Invoice must contain at least one row")
                .Must(rows => rows.Count <= 100)
                .WithMessage("Invoice cannot have more than 100 rows");
            RuleForEach(x => x.Rows).SetValidator(new CreateInvoiceRowDtoValidator());
        }
    }
}