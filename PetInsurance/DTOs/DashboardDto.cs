using System;
using System.Collections.Generic;

namespace PetInsurance.DTOs;

public class DashboardDto
{
    public int TotalQuotes { get; set; }
    public int ActiveQuotes { get; set; }
    public int ExpiredQuotes { get; set; }
    public int ConvertedQuotes { get; set; }
    public int CancelledQuotes { get; set; }
    public decimal AveragePremium { get; set; }
    public int ExpiringSoonCount { get; set; }
    public Dictionary<string, int> QuotesByStatus { get; set; } = new();
    public Dictionary<string, int> QuotesBySpecies { get; set; } = new();
}
