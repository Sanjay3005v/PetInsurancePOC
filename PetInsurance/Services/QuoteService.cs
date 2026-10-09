using System;
using Microsoft.Extensions.Options;
using PetInsurance.DTOs;
using PetInsurance.Exceptions;
using PetInsurance.Models;
using PetInsurance.Repositories.Interfaces;
using PetInsurance.Services.Interfaces;
using PetInsurance.Settings;

namespace PetInsurance.Services;

public class QuoteService : IQuoteService
{
    private readonly ICustomerRepository _customerRepository;
 
    private readonly IPetRepository _petRepository;
     
    private readonly IQuoteRepository _quoteRepository;
     
    private readonly IQuoteCoverageRepository _quoteCoverageRepository;
     
    private readonly IPremiumCalculatorService _premiumCalculatorService;
     
    private readonly PremiumSettings _settings;
     
    private readonly ILogger<QuoteService> _logger;
 
    public QuoteService(
        ICustomerRepository customerRepository,
        IPetRepository petRepository,
        IQuoteRepository quoteRepository,
        IQuoteCoverageRepository quoteCoverageRepository,
        IPremiumCalculatorService premiumCalculatorService,
        IOptions<PremiumSettings> settings,
        ILogger<QuoteService> logger)
        {
            _customerRepository = customerRepository;
            _petRepository = petRepository;
            _quoteRepository = quoteRepository;
            _quoteCoverageRepository = quoteCoverageRepository;
            _premiumCalculatorService = premiumCalculatorService;
            _settings = settings.Value;
            _logger = logger;
        }
         
    public async Task<int> CreateQuoteAsync(CreateQuoteDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Email))
        {
            throw new BusinessRuleException("Customer email is required.");
        }
         
        if (string.IsNullOrWhiteSpace(dto.PetName))
        {
            throw new BusinessRuleException("Pet name is required.");
        }
         
        int petAge = DateTime.Today.Year - dto.DateOfBirth.Year;
         
        if (petAge <= 0)
        {
            throw new BusinessRuleException("Pet age must be greater than zero.");
        }
         
        if (petAge > _settings.MaxEligibleAge)
        {
            throw new BusinessRuleException("Pet is not eligible.");
        }
         
        var customer = await _customerRepository.GetCustomerByEmailAsync(dto.Email);
         
        if (customer is null)
        {
            customer = new Customer
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            Phone = dto.Phone,
            ZipCode = dto.ZipCode
        };
         
            await _customerRepository.AddCustomerAsync(customer);
            await _customerRepository.SaveCustomerChangesAsync();
        }
         
        var pet = await _petRepository.GetPetByCustomerAndNameAsync( customer.CustomerId, dto.PetName);
         
        if (pet is null)
        {
            pet = new Pet
        {
            CustomerId = customer.CustomerId,
            PetName = dto.PetName,
            Species = dto.Species,
            Breed = dto.Breed,
            DateOfBirth = dto.DateOfBirth,
            Gender = dto.Gender,
            HasPreExistingCondition = dto.HasPreExistingCondition
        };
         
            await _petRepository.AddPetAsync(pet);
            await _petRepository.SavePetChangesAsync();
        }
         
        bool duplicate = await _quoteRepository 
            .HasDuplicateActiveQuoteAsync(
            customer.CustomerId,
            pet.PetId,
            dto.AnnualLimit,
            dto.Deductible,
            dto.ReimbursementPct,
            dto.Wellness);
         
        if (duplicate)
        {
            throw new BusinessRuleException("Duplicate active quote exists.");
        }
         
        var premium = _premiumCalculatorService.CalculatePremium(petAge,dto.Wellness,false);
         
        var quote = new Quote
        {
            QuoteNumber =$"QT-{Guid.NewGuid():N}"[..10],
             
            CustomerId = customer.CustomerId,
             
            PetId = pet.PetId,
             
            CreatedDate = DateTime.UtcNow,
             
            ExpiryDate = DateTime.UtcNow.AddYears(1),
             
            Status = QuoteStatus.Active,
             
            BasePremium = premium.BasePremium,
             
            AgeAdjustment = premium.AgeAdjustment,
             
            WellnessAmount = premium.WellnessAmount,
             
            DiscountAmount = premium.DiscountAmount,
             
            FinalPremium = premium.FinalPremium
        };
         
        await _quoteRepository.AddQuoteAsync(quote);
        await _quoteRepository.SaveQuoteChangesAsync();
         
        var coverage = new QuoteCoverage
        {
            QuoteId = quote.QuoteId,
            AnnualLimit = dto.AnnualLimit,
            Deductible = dto.Deductible,
            ReimbursementPct = dto.ReimbursementPct,
            Wellness = dto.Wellness
        };
        
        await _quoteCoverageRepository.AddQuoteAsync(coverage);
        
        await _quoteCoverageRepository.SaveQuoteChangesAsync();
        
        _logger.LogInformation("Quote created {QuoteNumber}",quote.QuoteNumber);
        
        return quote.QuoteId;
    }
    
    public async Task<PagedResultDto<QuoteListDto>> GetQuotesAsync(SearchQuoteDto dto)
    {
        var (quotes, totalCount) = await _quoteRepository.SearchQuotesAsync(dto);

        var items = quotes.Select(q => new QuoteListDto
        {
            QuoteId = q.QuoteId,
            QuoteNumber = q.QuoteNumber,
            CustomerName = q.Customer != null ? $"{q.Customer.FirstName} {q.Customer.LastName}".Trim() : string.Empty,
            PetName = q.Pet?.PetName ?? string.Empty,
            Species = q.Pet?.Species ?? string.Empty,
            FinalPremium = q.FinalPremium,
            ExpiryDate = q.ExpiryDate,
            Status = q.Status.ToString()
        }).ToList();

        return new PagedResultDto<QuoteListDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = dto.PageNumber > 0 ? dto.PageNumber : 1,
            PageSize = dto.PageSize > 0 ? dto.PageSize : 10
        };
    }
     
    public async Task<QuoteDetailsDto?> GetQuoteByIdAsync(int quoteId)
    {
        var quote = await _quoteRepository.GetQuoteByIdAsync(quoteId);
         
        if (quote is null)
        {
            return null;
        }
         
        return new QuoteDetailsDto
        {
            QuoteId = quote.QuoteId,
            QuoteNumber = quote.QuoteNumber,
            CustomerName = $"{quote.Customer?.FirstName} {quote.Customer?.LastName}",
            FirstName = quote.Customer?.FirstName ?? "",
            LastName = quote.Customer?.LastName ?? "",
            Phone = quote.Customer?.Phone ?? "",
            Email = quote.Customer?.Email ?? "",
            ZipCode = quote.Customer?.ZipCode ?? "",
            PetName = quote.Pet?.PetName ?? "",
            Species = quote.Pet?.Species ?? "",
            Breed = quote.Pet?.Breed ?? "",
            DateOfBirth = quote.Pet?.DateOfBirth,
            Gender = quote.Pet?.Gender ?? "" , 
            HasPreExistingCondition = quote.Pet?.HasPreExistingCondition ?? false,
            CreatedDate = quote.CreatedDate,
            ExpiryDate = quote.ExpiryDate,
            Status = quote.Status.ToString(),
            AnnualLimit = quote.QuoteCoverage?.AnnualLimit ?? 0,
            Deductible = quote.QuoteCoverage?.Deductible ?? 0,
            ReimbursementPct = quote.QuoteCoverage?.ReimbursementPct ?? 0,
            Wellness = quote.QuoteCoverage?.Wellness ?? true, 
            BasePremium = quote.BasePremium,
            AgeAdjustment = quote.AgeAdjustment,
            WellnessAmount = quote.WellnessAmount,
            DiscountAmount = quote.DiscountAmount,
            FinalPremium = quote.FinalPremium
        };
    }
     
    public async Task<bool> UpdateQuoteAsync(int quoteId, UpdateQuoteDto dto)
    {
        var quote = await _quoteRepository.GetQuoteByIdAsync(quoteId);

        if (quote is null)
        {
            return false;
        }

        if (quote.Status == QuoteStatus.Expired || quote.Status == QuoteStatus.Cancelled || quote.Status == QuoteStatus.Converted)
        {
            throw new BusinessRuleException("An expired, cancelled, or converted quote cannot be modified.");
        }

        if (quote.Customer != null)
        {
            if (!string.IsNullOrWhiteSpace(dto.FirstName)) quote.Customer.FirstName = dto.FirstName;
            if (!string.IsNullOrWhiteSpace(dto.LastName)) quote.Customer.LastName = dto.LastName;
            if (!string.IsNullOrWhiteSpace(dto.Phone)) quote.Customer.Phone = dto.Phone;
            if (!string.IsNullOrWhiteSpace(dto.ZipCode)) quote.Customer.ZipCode = dto.ZipCode;
        }

        if (quote.Pet != null)
        {
            if (!string.IsNullOrWhiteSpace(dto.PetName)) quote.Pet.PetName = dto.PetName;
            if (!string.IsNullOrWhiteSpace(dto.Species)) quote.Pet.Species = dto.Species;
            if (!string.IsNullOrWhiteSpace(dto.Breed)) quote.Pet.Breed = dto.Breed;
            if (dto.DateOfBirth.HasValue) quote.Pet.DateOfBirth = dto.DateOfBirth.Value;
            if (!string.IsNullOrWhiteSpace(dto.Gender)) quote.Pet.Gender = dto.Gender;
            if (dto.HasPreExistingCondition.HasValue) quote.Pet.HasPreExistingCondition = dto.HasPreExistingCondition.Value;
        }

        if (quote.QuoteCoverage != null)
        {
            quote.QuoteCoverage.AnnualLimit = dto.AnnualLimit > 0 ? dto.AnnualLimit : quote.QuoteCoverage.AnnualLimit;
            quote.QuoteCoverage.Deductible = dto.Deductible > 0 ? dto.Deductible : quote.QuoteCoverage.Deductible;
            quote.QuoteCoverage.ReimbursementPct = dto.ReimbursementPct > 0 ? dto.ReimbursementPct : quote.QuoteCoverage.ReimbursementPct;
            quote.QuoteCoverage.Wellness = dto.Wellness;
        }

        int petAge = 1;

        if (quote.Pet != null)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);

            petAge = today.Year - quote.Pet.DateOfBirth.Year;

            if (quote.Pet.DateOfBirth > today.AddYears(-petAge))
            {
                petAge--;
            }
        }
        bool wellness = quote.QuoteCoverage?.Wellness ?? dto.Wellness;
        var premium = _premiumCalculatorService.CalculatePremium(petAge, wellness, false);

        quote.BasePremium = premium.BasePremium;
        quote.AgeAdjustment = premium.AgeAdjustment;
        quote.WellnessAmount = premium.WellnessAmount;
        quote.DiscountAmount = premium.DiscountAmount;
        quote.FinalPremium = premium.FinalPremium;

        _quoteRepository.UpdateQuote(quote);
        await _quoteRepository.SaveQuoteChangesAsync();

        return true;
    }
     
    public async Task<bool> ConvertQuoteAsync(int quoteId)
    {
        var quote = await _quoteRepository.GetQuoteByIdAsync(quoteId);
         
        if (quote is null)
        {
            return false;
        }
         
        if (quote.Status == QuoteStatus.Expired || quote.Status == QuoteStatus.Cancelled || quote.Status == QuoteStatus.Converted)
        {
            throw new BusinessRuleException("Quote cannot be converted.");
        }
         
        quote.Status = QuoteStatus.Converted;
         
        _quoteRepository.UpdateQuote(quote);
         
        await _quoteRepository.SaveQuoteChangesAsync();
         
        return true;
    }
     
    public async Task<bool> CancelQuoteAsync(int quoteId)
    {
        var quote = await _quoteRepository.GetQuoteByIdAsync(quoteId);
         
        if (quote is null)
        {
            return false;
        }
         
        quote.Status = QuoteStatus.Cancelled;
         
        _quoteRepository.UpdateQuote(quote);
         
        await _quoteRepository.SaveQuoteChangesAsync();
         
        return true;
    }
     
    public async Task<bool> RecalculateQuoteAsync(int quoteId, bool applyDiscount)
    {
        var quote = await _quoteRepository.GetQuoteByIdAsync(quoteId);
        
        if (quote is null)
        {
            return false;
        }
        
        if (quote.Pet is null || quote.QuoteCoverage is null)
        {
            return false;
        }
    
        int petAge = DateTime.Today.Year - quote.Pet.DateOfBirth.Year;
        
        // Toggle discount if not specified, or use specified applyDiscount value
        bool effectiveDiscount = applyDiscount || quote.DiscountAmount == 0;
        
        var premium = _premiumCalculatorService.CalculatePremium(petAge, quote.QuoteCoverage.Wellness, effectiveDiscount);
        
        quote.BasePremium = premium.BasePremium;
        quote.AgeAdjustment = premium.AgeAdjustment;
        quote.WellnessAmount = premium.WellnessAmount;
        quote.DiscountAmount = premium.DiscountAmount;
        quote.FinalPremium = premium.FinalPremium;
        
        _quoteRepository.UpdateQuote(quote);
        await _quoteRepository.SaveQuoteChangesAsync();
        
        return true;
    }

    public async Task<DashboardDto> GetDashboardMetricsAsync()
    {
        return await _quoteRepository.GetDashboardMetricsAsync();
    }
}
