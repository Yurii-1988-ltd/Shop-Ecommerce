
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Ecommerce.Currency.Modules.Infrastructure.Data;

public sealed class ExchangeRateContextFactory
    : IDesignTimeDbContextFactory<ExchangeRateContext>
{
    public ExchangeRateContext CreateDbContext(string[] args)
    {
        var optionsBuilder =
            new DbContextOptionsBuilder<ExchangeRateContext>();

        optionsBuilder.UseNpgsql(
     "Host=localhost;Port=5432;Database=currencies;Username=postgres;Password=postgres");

        return new ExchangeRateContext(optionsBuilder.Options);
    }
}