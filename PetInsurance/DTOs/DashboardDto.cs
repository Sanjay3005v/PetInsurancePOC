using System;

namespace PetInsurance.DTOs;

public class DashboardDto
{
    public int TotalQuotes { get; set; }
    public int ActiveQuotes { get; set; }
    public int ExpiredQuotes { get; set; }
    public int ConvertedQuotes { get; set; }
    public decimal AveragePremium { get; set; }
}
