using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PetInsurance.Models;

namespace PetInsurance.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        await context.Database.EnsureCreatedAsync();

        // 1. Seed Default User if empty
        if (!await context.Users.AnyAsync())
        {
            var adminUser = new AppUser
            {
                UserName = "admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!")
            };
            await context.Users.AddAsync(adminUser);
            await context.SaveChangesAsync();
        }

        // 2. Seed Sample Quotes if empty
        if (!await context.Quotes.AnyAsync())
        {
            var customer1 = new Customer
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                Phone = "555-0192",
                ZipCode = "90210"
            };

            var customer2 = new Customer
            {
                FirstName = "Jane",
                LastName = "Smith",
                Email = "jane.smith@example.com",
                Phone = "555-0143",
                ZipCode = "10001"
            };

            await context.Customers.AddRangeAsync(customer1, customer2);
            await context.SaveChangesAsync();

            var pet1 = new Pet
            {
                CustomerId = customer1.CustomerId,
                PetName = "Buddy",
                Species = "Dog",
                Breed = "Golden Retriever",
                DateOfBirth = DateTime.Today.AddYears(-3),
                Gender = "Male",
                HasPreExistingCondition = false
            };

            var pet2 = new Pet
            {
                CustomerId = customer2.CustomerId,
                PetName = "Whiskers",
                Species = "Cat",
                Breed = "Siamese",
                DateOfBirth = DateTime.Today.AddYears(-8),
                Gender = "Female",
                HasPreExistingCondition = true
            };

            await context.Pets.AddRangeAsync(pet1, pet2);
            await context.SaveChangesAsync();

            var quote1 = new Quote
            {
                QuoteNumber = "QT-A1000001",
                CustomerId = customer1.CustomerId,
                PetId = pet1.PetId,
                CreatedDate = DateTime.UtcNow.AddDays(-5),
                ExpiryDate = DateTime.UtcNow.AddDays(25),
                Status = QuoteStatus.Active,
                BasePremium = 50.00m,
                AgeAdjustment = 0.00m,
                WellnessAmount = 15.00m,
                DiscountAmount = 0.00m,
                FinalPremium = 65.00m
            };

            var quote2 = new Quote
            {
                QuoteNumber = "QT-B2000002",
                CustomerId = customer2.CustomerId,
                PetId = pet2.PetId,
                CreatedDate = DateTime.UtcNow.AddDays(-15),
                ExpiryDate = DateTime.UtcNow.AddDays(15),
                Status = QuoteStatus.Converted,
                BasePremium = 50.00m,
                AgeAdjustment = 20.00m,
                WellnessAmount = 0.00m,
                DiscountAmount = 5.00m,
                FinalPremium = 65.00m
            };

            await context.Quotes.AddRangeAsync(quote1, quote2);
            await context.SaveChangesAsync();

            var coverage1 = new QuoteCoverage
            {
                QuoteId = quote1.QuoteId,
                AnnualLimit = 5000.00m,
                Deductible = 250.00m,
                ReimbursementPct = 80.00m,
                Wellness = true
            };

            var coverage2 = new QuoteCoverage
            {
                QuoteId = quote2.QuoteId,
                AnnualLimit = 10000.00m,
                Deductible = 100.00m,
                ReimbursementPct = 90.00m,
                Wellness = false
            };

            await context.QuoteCoverages.AddRangeAsync(coverage1, coverage2);
            await context.SaveChangesAsync();
        }
    }
}
