

using Ecommerce.Currency.Modules.Domain.Entities;
using Ecommerce.Currency.Modules.Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Currency.Modules.Infrastructure.Data;

public sealed class ExchangeRateContext : DbContext
{
    public ExchangeRateContext(DbContextOptions<ExchangeRateContext>options):base(options)
    {
        
    }
    public DbSet<ExchangeRate> ExchangeRates => Set<ExchangeRate>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ExchangeRateConfiguration).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
