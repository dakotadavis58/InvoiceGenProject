using FluentValidation;
using InvoiceGeneratorAPI.DTOs.Invoice;

namespace InvoiceGeneratorAPI.Validators;

public class CreateInvoiceRequestValidator : AbstractValidator<CreateInvoiceRequest>
{
    public CreateInvoiceRequestValidator()
    {
        RuleFor(x => x.ClientId)
            .NotNull()
            .When(x => string.IsNullOrEmpty(x.ClientName) && string.IsNullOrEmpty(x.ClientEmail))
            .WithMessage("Either ClientId or ClientName/ClientEmail must be provided");

        RuleFor(x => x.ClientName)
            .NotEmpty()
            .MaximumLength(100)
            .When(x => !x.ClientId.HasValue)
            .WithMessage("Client name is required when no ClientId is provided");

        RuleFor(x => x.ClientEmail)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(100)
            .When(x => !x.ClientId.HasValue)
            .WithMessage("Valid client email is required when no ClientId is provided");

        RuleFor(x => x.IssueDate)
            .NotEmpty()
            .Must(date => date.Date <= DateTime.UtcNow.Date)
            .WithMessage("Issue date cannot be in the future");

        RuleFor(x => x.DueDate)
            .NotEmpty()
            .Must((invoice, dueDate) => dueDate.Date >= invoice.IssueDate.Date)
            .WithMessage("Due date must be after or equal to issue date");

        RuleFor(x => x.TaxRate)
            .InclusiveBetween(0, 100)
            .WithMessage("Tax rate must be between 0 and 100");

        RuleFor(x => x.Items)
            .NotEmpty()
            .WithMessage("At least one item is required");

        RuleForEach(x => x.Items).SetValidator(new CreateInvoiceItemRequestValidator());
    }
}

public class CreateInvoiceItemRequestValidator : AbstractValidator<CreateInvoiceItemRequest>
{
    public CreateInvoiceItemRequestValidator()
    {
        RuleFor(x => x.Description)
            .NotEmpty()
            .MaximumLength(200)
            .WithMessage("Description is required and must not exceed 200 characters");

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than 0");

        RuleFor(x => x.UnitPrice)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Unit price must be greater than or equal to 0");
    }
}

public class UpdateInvoiceRequestValidator : AbstractValidator<UpdateInvoiceRequest>
{
    public UpdateInvoiceRequestValidator()
    {
        When(x => x.ClientId.HasValue, () =>
        {
            RuleFor(x => x.ClientName).Null();
            RuleFor(x => x.ClientEmail).Null();
        });

        When(x => x.ClientName != null || x.ClientEmail != null, () =>
        {
            RuleFor(x => x.ClientId).Null();
        });

        When(x => x.IssueDate.HasValue, () =>
        {
            RuleFor(x => x.IssueDate)
                .Must(date => date!.Value.Date <= DateTime.UtcNow.Date)
                .WithMessage("Issue date cannot be in the future");
        });

        When(x => x.DueDate.HasValue, () =>
        {
            RuleFor(x => x.DueDate)
                .Must((invoice, dueDate) =>
                    !invoice.IssueDate.HasValue || dueDate!.Value.Date >= invoice.IssueDate.Value.Date)
                .WithMessage("Due date must be after or equal to issue date");
        });

        When(x => x.TaxRate.HasValue, () =>
        {
            RuleFor(x => x.TaxRate)
                .InclusiveBetween(0, 100)
                .WithMessage("Tax rate must be between 0 and 100");
        });

        When(x => x.Items != null, () =>
        {
            RuleFor(x => x.Items)
                .NotEmpty()
                .WithMessage("If items are provided, at least one item is required");

            RuleForEach(x => x.Items).SetValidator(new CreateInvoiceItemRequestValidator());
        });

        RuleFor(x => x.Status)
            .Must(status => status == null ||
                new[] { "Draft", "Sent", "Paid", "Overdue", "Cancelled" }.Contains(status))
            .WithMessage("Invalid status value");
    }
}