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
         
        var customer = await _customerRepository.GetByEmailAsync(dto.Email);
         
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
         
            await _customerRepository.AddAsync(customer);
            await _customerRepository.SaveChangesAsync();
        }
         
        var pet = await _petRepository.GetByCustomerAndNameAsync( customer.CustomerId, dto.PetName);
         
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
         
            await _petRepository.AddAsync(pet);
            await _petRepository.SaveChangesAsync();
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
             
            ExpiryDate = DateTime.UtcNow.AddDays(30),
             
            Status = QuoteStatus.Active,
             
            BasePremium = premium.BasePremium,
             
            AgeAdjustment = premium.AgeAdjustment,
             
            WellnessAmount = premium.WellnessAmount,
             
            DiscountAmount = premium.DiscountAmount,
             
            FinalPremium = premium.FinalPremium
        };
         
        await _quoteRepository.AddAsync(quote);
        await _quoteRepository.SaveChangesAsync();
         
        var coverage = new QuoteCoverage
        {
            QuoteId = quote.QuoteId,
            AnnualLimit = dto.AnnualLimit,
            Deductible = dto.Deductible,
            ReimbursementPct = dto.ReimbursementPct,
            Wellness = dto.Wellness
        };
         
        await _quoteCoverageRepository.AddAsync(coverage);
         
        await _quoteCoverageRepository.SaveChangesAsync();
         
        _logger.LogInformation("Quote created {QuoteNumber}",quote.QuoteNumber);
         
        return quote.QuoteId;
    }
     
    public async Task<QuoteDetailsDto?> GetQuoteByIdAsync(int quoteId)
    {
        var quote = await _quoteRepository.GetByIdAsync(quoteId);
         
        if (quote is null)
        {
            return null;
        }
         
        return new QuoteDetailsDto
        {
            QuoteId = quote.QuoteId,
            QuoteNumber = quote.QuoteNumber,
            CustomerName = $"{quote.Customer?.FirstName} {quote.Customer?.LastName}",
            Email = quote.Customer?.Email ?? "",
            PetName = quote.Pet?.PetName ?? "",
            Species = quote.Pet?.Species ?? "",
            CreatedDate = quote.CreatedDate,
            ExpiryDate = quote.ExpiryDate,
            Status = quote.Status.ToString(),
            BasePremium = quote.BasePremium,
            AgeAdjustment = quote.AgeAdjustment,
            WellnessAmount = quote.WellnessAmount,
            DiscountAmount = quote.DiscountAmount,
            FinalPremium = quote.FinalPremium
        };
    }
     
    public async Task<bool> ConvertQuoteAsync(int quoteId)
    {
        var quote = await _quoteRepository.GetByIdAsync(quoteId);
         
        if (quote is null)
        {
            return false;
        }
         
        if (quote.Status == QuoteStatus.Expired || quote.Status == QuoteStatus.Cancelled || quote.Status == QuoteStatus.Converted)
        {
            throw new BusinessRuleException("Quote cannot be converted.");
        }
         
        quote.Status = QuoteStatus.Converted;
         
        _quoteRepository.Update(quote);
         
        await _quoteRepository.SaveChangesAsync();
         
        return true;
    }
     
    public async Task<bool> CancelQuoteAsync(int quoteId)
    {
        var quote = await _quoteRepository.GetByIdAsync(quoteId);
         
        if (quote is null)
        {
            return false;
        }
         
        quote.Status = QuoteStatus.Cancelled;
         
        _quoteRepository.Update(quote);
         
        await _quoteRepository.SaveChangesAsync();
         
        return true;
    }
     
    public async Task<bool> RecalculateQuoteAsync(int quoteId, bool applyDiscount)
    {
        var quote = await _quoteRepository.GetByIdAsync(quoteId);
         
        if (quote is null)
        {
            return false;
        }
         
        if (quote.Pet is null || quote.QuoteCoverage is null)
        {
            return false;
        }
     
        int petAge = DateTime.Today.Year - quote.Pet.DateOfBirth.Year;
         
        var premium = _premiumCalculatorService.CalculatePremium(petAge, quote.QuoteCoverage.Wellness, applyDiscount);
         
        quote.BasePremium = premium.BasePremium;
         
        quote.AgeAdjustment = premium.AgeAdjustment;
         
        quote.WellnessAmount = premium.WellnessAmount;
         
        quote.DiscountAmount = premium.DiscountAmount;
         
        quote.FinalPremium = premium.FinalPremium;
         
        _quoteRepository.Update(quote);
         
        await _quoteRepository.SaveChangesAsync();
         
        return true;
    }
}
