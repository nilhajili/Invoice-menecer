using WebApplication4.DTOs;
using FluentValidation;
namespace WebApplication4.Validators;

public class InvoiceQueryDtoValidator : AbstractValidator<InvoiceQueryDto>
{
    public InvoiceQueryDtoValidator()
    {
        RuleFor(x => x.Page).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
        RuleFor(x => x.FromDate)
            .LessThanOrEqualTo(x => x.ToDate)
            .When(x => x.FromDate.HasValue && x.ToDate.HasValue);
    }
}