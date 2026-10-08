using System;

namespace PetInsurance.DTOs;

public class SearchQuoteDto
{
    public string? SearchQuery { get; set; }
    public string? Email { get; set; }
    public string? PetName { get; set; }
    public string? Species { get; set; }
    public string? Status { get; set; }
    public decimal? MinPremium { get; set; }
    public decimal? MaxPremium { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
