using WebApplication4.DTOs;
using FluentValidation;

namespace WebApplication4.Validators;

public class InvoiceQueryDtoValidator : AbstractValidator<InvoiceQueryDto>
{
    private readonly string[] allowedOrderBy = { "TotalSum", "CreatedAt" };

    public InvoiceQueryDtoValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0)
            .WithMessage("Page must be greater than 0");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("PageSize must be between 1 and 100");

        RuleFor(x => x.FromDate)
            .LessThanOrEqualTo(x => x.ToDate)
            .When(x => x.FromDate.HasValue && x.ToDate.HasValue)
            .WithMessage("FromDate must be less than or equal to ToDate");

        RuleFor(x => x.OrderBy)
            .Must(x => string.IsNullOrEmpty(x) || allowedOrderBy.Contains(x))
            .WithMessage($"OrderBy must be one of: {string.Join(", ", allowedOrderBy)}");
    }
}