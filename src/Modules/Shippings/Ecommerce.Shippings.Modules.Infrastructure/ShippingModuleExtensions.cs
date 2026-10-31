using Ecommerce.Shippings.Modules.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;



namespace Ecommerce.Shippings.Modules.Infrastructure;

public static class ShippingModuleExtensions
{
    public static IServiceCollection AddShippingModule(
        this IServiceCollection services,
        IConfiguration config)
    {
        // MediatR


        // PostgreSQL
        var connectionString =
            config.GetConnectionString("shippings")
            ?? throw new InvalidOperationException(
                "Connection string 'shippings' is not configured.");

       
        services.AddDbContext<ShippingContext>(options =>
        {
            options.UseNpgsql(connectionString);
        });

        // MassTransit

        return services;
    }

    public static async Task ApplyShippingMigrationsAsync(
        this IHost host)
    {
        using var scope = host.Services.CreateScope();

        var context = scope.ServiceProvider
            .GetRequiredService<ShippingContext>();

        await context.Database.MigrateAsync();
    }
}