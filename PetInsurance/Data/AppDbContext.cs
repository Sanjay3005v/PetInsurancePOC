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
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

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
