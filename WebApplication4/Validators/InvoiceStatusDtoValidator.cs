using FluentValidation;
using WebApplication4.DTOs;
using WebApplication4.Models;

namespace WebApplication4.Validators;

public class InvoiceStatusDtoValidator : AbstractValidator<InvoiceStatusDto>
{
    private readonly InvoiceStatus[] allowedStatuses = 
    { InvoiceStatus.Created, InvoiceStatus.Sent, InvoiceStatus.Received,
        InvoiceStatus.Paid, InvoiceStatus.Cancelled, InvoiceStatus.Rejected };

    public InvoiceStatusDtoValidator()
    {
        RuleFor(x => x.Status)
            .Must(s => allowedStatuses.Contains(s))
            .WithMessage("Invalid invoice status.");
    }
}