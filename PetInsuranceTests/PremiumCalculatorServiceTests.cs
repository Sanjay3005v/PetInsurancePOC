using FluentAssertions;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using PetInsurance.Services;
using PetInsurance.Settings;

namespace PetInsuranceTests;

[TestFixture]
public class PremiumCalculatorServiceTests
{
    private PremiumCalculatorService _calculator = null!;

    [SetUp]
    public void Setup()
    {
        var settings = Options.Create(new PremiumSettings
        {
            BasePremium = 50.00m,
            SeniorPetAge = 7,
            SeniorPetAdjustment = 20.00m,
            WellnessAmount = 15.00m,
            DiscountAmount = 10.00m,
            MaxEligibleAge = 15
        });

        _calculator = new PremiumCalculatorService(settings);
    }

    [Test]
    public void CalculatePremium_StandardPet_ReturnsBasePremium()
    {
        // Arrange & Act
        var result = _calculator.CalculatePremium(petAge: 3, wellnessSelected: false, applyDiscount: false);

        // Assert
        result.BasePremium.Should().Be(50.00m);
        result.AgeAdjustment.Should().Be(0.00m);
        result.WellnessAmount.Should().Be(0.00m);
        result.DiscountAmount.Should().Be(0.00m);
        result.FinalPremium.Should().Be(50.00m);
    }

    [Test]
    public void CalculatePremium_SeniorPetWithWellnessAndDiscount_AppliesAllAdjustments()
    {
        // Arrange & Act
        var result = _calculator.CalculatePremium(petAge: 8, wellnessSelected: true, applyDiscount: true);

        // Assert: 50 (base) + 20 (senior) + 15 (wellness) - 10 (discount) = 75
        result.BasePremium.Should().Be(50.00m);
        result.AgeAdjustment.Should().Be(20.00m);
        result.WellnessAmount.Should().Be(15.00m);
        result.DiscountAmount.Should().Be(10.00m);
        result.FinalPremium.Should().Be(75.00m);
    }

    [Test]
    public void CalculatePremium_HighDiscount_FinalPremiumCannotBeNegative()
    {
        // Arrange
        var settingsHighDiscount = Options.Create(new PremiumSettings
        {
            BasePremium = 50.00m,
            SeniorPetAge = 7,
            SeniorPetAdjustment = 0.00m,
            WellnessAmount = 0.00m,
            DiscountAmount = 100.00m,
            MaxEligibleAge = 15
        });
        var calculator = new PremiumCalculatorService(settingsHighDiscount);

        // Act
        var result = calculator.CalculatePremium(petAge: 2, wellnessSelected: false, applyDiscount: true);

        // Assert: 50 - 100 = -50, max(0, -50) = 0
        result.FinalPremium.Should().Be(0.00m);
    }
}
