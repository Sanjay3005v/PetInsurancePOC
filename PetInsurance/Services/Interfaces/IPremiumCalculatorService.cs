using System;

namespace PetInsurance.Services.Interfaces;

public interface IPremiumCalculatorService
{
    (
        decimal BasePremium,
        decimal AgeAdjustment,
        decimal WellnessAmount,
        decimal DiscountAmount,
        decimal FinalPremium
    ) CalculatePremium(
        int petAge,
        bool wellnessSelected,
        bool applyDiscount);
}
