using System;

namespace PetInsurance.Settings;

public class PremiumSettings
{
    public int MaxEligibleAge { get; set; }
    public decimal BasePremium { get; set; }
    public int SeniorPetAge { get; set; }
    public decimal SeniorPetAdjustment { get; set; }
    public decimal WellnessAmount { get; set; }
    public decimal DiscountAmount { get; set; }
}
