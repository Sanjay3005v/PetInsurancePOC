using System;
using Microsoft.Extensions.Options;
using PetInsurance.Services.Interfaces;
using PetInsurance.Settings;

namespace PetInsurance.Services;

public class PremiumCalculatorService : IPremiumCalculatorService
{
    private readonly PremiumSettings _settings;
 
    public PremiumCalculatorService(IOptions<PremiumSettings> settings)
    {
        _settings = settings.Value;
    }
     
    public (
        decimal BasePremium,
        decimal AgeAdjustment,
        decimal WellnessAmount,
        decimal DiscountAmount,
        decimal FinalPremium
    ) CalculatePremium(
        int petAge,
        bool wellnessSelected,
        bool applyDiscount)
    {
        decimal basePremium = _settings.BasePremium;
         
        decimal ageAdjustment = 0;
         
        decimal wellnessAmount = 0;
         
        decimal discountAmount = 0;
         
        if (petAge >= _settings.SeniorPetAge)
        {
            ageAdjustment = _settings.SeniorPetAdjustment;
        }
         
        if (wellnessSelected)
        {
            wellnessAmount = _settings.WellnessAmount;
        }
         
        if (applyDiscount)
        {
            discountAmount = _settings.DiscountAmount;
        }
         
        decimal finalPremium =basePremium + ageAdjustment + wellnessAmount - discountAmount;
         
        finalPremium = Math.Max(0, finalPremium);
         
        return (
            basePremium,
            ageAdjustment,
            wellnessAmount,
            discountAmount,
            finalPremium);
    }
}
