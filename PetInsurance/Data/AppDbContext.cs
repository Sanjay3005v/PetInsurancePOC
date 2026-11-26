using System;
using Microsoft.EntityFrameworkCore;
using PetInsurance.Models;

namespace PetInsurance.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }

    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Pet> Pets => Set<Pet>();
    public DbSet<Quote> Quotes => Set<Quote>();
    public DbSet<QuoteCoverage> QuoteCoverages => Set<QuoteCoverage>();
    public DbSet<AppUser> Users => Set<AppUser>();
    public DbSet<Address> Addresses => Set<Address>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Map AppUser explicitly to the database table "Users"
        modelBuilder.Entity<AppUser>().ToTable("Users");
        modelBuilder.Entity<Customer>().ToTable("Customers");
        modelBuilder.Entity<Pet>().ToTable("Pets");
        modelBuilder.Entity<Quote>().ToTable("Quotes");
        modelBuilder.Entity<QuoteCoverage>().ToTable("QuoteCoverages");
        modelBuilder.Entity<Address>().ToTable("Addresses");

        // Customer -> Addresses (1:M)
        modelBuilder.Entity<Customer>()
            .HasMany(c => c.Addresses)
            .WithOne(a => a.Customer)
            .HasForeignKey(a => a.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        // Convert DateOnly for Pet.DateOfBirth to handle Postgres timestamp/date smoothly
        modelBuilder.Entity<Pet>()
            .Property(p => p.DateOfBirth)
            .HasConversion(
                d => d.ToDateTime(TimeOnly.MinValue),
                dt => DateOnly.FromDateTime(dt));

        // Customer -> Pets (1:M)
        modelBuilder.Entity<Customer>()
        .HasMany(c => c.Pets)
        .WithOne(p => p.Customer)
        .HasForeignKey(p => p.CustomerId);
         
        // Customer -> Quotes (1:M)
        modelBuilder.Entity<Customer>()
        .HasMany(c => c.Quotes)
        .WithOne(q => q.Customer)
        .HasForeignKey(q => q.CustomerId);
         
        // Pet -> Quotes (1:M)
        modelBuilder.Entity<Pet>()
        .HasMany(p => p.Quotes)
        .WithOne(q => q.Pet)
        .HasForeignKey(q => q.PetId);
         
        // Quote -> QuoteCoverage (1:1)
        modelBuilder.Entity<Quote>()
        .HasOne(q => q.QuoteCoverage)
        .WithOne(qc => qc.Quote)
        .HasForeignKey<QuoteCoverage>(qc => qc.QuoteId);
    }

}
