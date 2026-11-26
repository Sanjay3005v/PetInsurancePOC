using FluentValidation;
using PetInsurance.DTOs;

namespace PetInsurance.Validators;

public class CreateQuoteDtoValidator : AbstractValidator<CreateQuoteDto>
{
    public CreateQuoteDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Customer email is required.")
            .EmailAddress().WithMessage("Please provide a valid email address.");

        RuleFor(x => x.PetName)
            .NotEmpty().WithMessage("Pet name is required.");
    }
}
