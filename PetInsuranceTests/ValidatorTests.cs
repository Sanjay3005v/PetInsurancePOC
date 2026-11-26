using FluentAssertions;
using NUnit.Framework;
using PetInsurance.DTOs;
using PetInsurance.Validators;

namespace PetInsuranceTests;

[TestFixture]
public class ValidatorTests
{
    private CreateQuoteDtoValidator _validator = null!;

    [SetUp]
    public void Setup()
    {
        _validator = new CreateQuoteDtoValidator();
    }

    [Test]
    public void CreateQuoteDtoValidator_ValidDto_PassesValidation()
    {
        var dto = new CreateQuoteDto
        {
            Email = "john@example.com",
            PetName = "Buddy"
        };

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeTrue();
    }

    [Test]
    public void CreateQuoteDtoValidator_MissingEmailAndPetName_FailsValidation()
    {
        var dto = new CreateQuoteDto
        {
            Email = "",
            PetName = ""
        };

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateQuoteDto.Email));
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateQuoteDto.PetName));
    }
}
