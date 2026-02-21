using WebApplication4.DTOs;
using FluentValidation;

namespace WebApplication4.Validators;

public class CustomerQueryDtoValidator : AbstractValidator<CustomerQueryDto>
{
    private readonly string[] allowedOrderBy = { "Name", "CreatedAt" };

    public CustomerQueryDtoValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0)
            .WithMessage("Page must be greater than 0");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("PageSize must be between 1 and 100");

        RuleFor(x => x.OrderBy)
            .Must(x => string.IsNullOrEmpty(x) || allowedOrderBy.Contains(x))
            .WithMessage($"OrderBy must be one of: {string.Join(", ", allowedOrderBy)}");
    }
}