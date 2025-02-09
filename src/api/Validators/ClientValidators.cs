using FluentValidation;
using InvoiceGeneratorAPI.DTOs.Client;

namespace InvoiceGeneratorAPI.Validators;

public class CreateClientRequestValidator : AbstractValidator<CreateClientRequest>
{
    public CreateClientRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100)
            .WithMessage("Name is required and must not exceed 100 characters");

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(100)
            .WithMessage("A valid email address is required");

        RuleFor(x => x.Phone)
            .MaximumLength(20)
            .When(x => x.Phone != null)
            .WithMessage("Phone number must not exceed 20 characters");

        RuleFor(x => x.AddressLine1)
            .MaximumLength(100)
            .When(x => x.AddressLine1 != null)
            .WithMessage("Address line 1 must not exceed 100 characters");

        RuleFor(x => x.AddressLine2)
            .MaximumLength(100)
            .When(x => x.AddressLine2 != null)
            .WithMessage("Address line 2 must not exceed 100 characters");

        RuleFor(x => x.City)
            .MaximumLength(50)
            .When(x => x.City != null)
            .WithMessage("City must not exceed 50 characters");

        RuleFor(x => x.State)
            .MaximumLength(50)
            .When(x => x.State != null)
            .WithMessage("State must not exceed 50 characters");

        RuleFor(x => x.PostalCode)
            .MaximumLength(20)
            .When(x => x.PostalCode != null)
            .WithMessage("Postal code must not exceed 20 characters");

        RuleFor(x => x.Country)
            .MaximumLength(50)
            .When(x => x.Country != null)
            .WithMessage("Country must not exceed 50 characters");

        RuleFor(x => x.TaxNumber)
            .MaximumLength(50)
            .When(x => x.TaxNumber != null)
            .WithMessage("Tax number must not exceed 50 characters");

        RuleFor(x => x.Notes)
            .MaximumLength(500)
            .When(x => x.Notes != null)
            .WithMessage("Notes must not exceed 500 characters");
    }
}

public class UpdateClientRequestValidator : AbstractValidator<UpdateClientRequest>
{
    public UpdateClientRequestValidator()
    {
        RuleFor(x => x.Name)
            .MaximumLength(100)
            .When(x => x.Name != null)
            .WithMessage("Name must not exceed 100 characters");

        RuleFor(x => x.Email)
            .EmailAddress()
            .MaximumLength(100)
            .When(x => x.Email != null)
            .WithMessage("Email must be a valid email address");

        // ... same rules as CreateClientRequestValidator for other fields ...
    }
}