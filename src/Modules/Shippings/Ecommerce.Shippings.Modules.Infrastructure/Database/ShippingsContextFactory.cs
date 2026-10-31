

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Ecommerce.Shippings.Modules.Infrastructure.Database;

internal sealed class ShippingsContextFactory : IDesignTimeDbContextFactory<ShippingContext>
{
    public ShippingContext CreateDbContext(string[] args)
    {
        var optionsBuilder =
           new DbContextOptionsBuilder<ShippingContext>();

        optionsBuilder.UseNpgsql(
     "Host=localhost;Port=5432;Database=shippings;Username=postgres;Password=postgres");

        return new ShippingContext(optionsBuilder.Options);
    }
}
