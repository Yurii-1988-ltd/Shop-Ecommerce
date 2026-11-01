
namespace Ecommerce.Shippings.Modules.Infrastructure;

public static class ShippingModuleExtensions
{
    public static IServiceCollection AddShippingModule(
        this IServiceCollection services,
        IConfiguration config)
    {
        // MediatR
        services.AddMediatR(options =>
        {
            options.RegisterServicesFromAssembly(
                typeof(CreateShippingMethodCommandHandler).Assembly);
        });

        // PostgreSQL
        var connectionString = config.GetConnectionString("shippings")
                               ?? throw new InvalidOperationException(
                                   "Connection string 'shippings' is not configured.");

        services.AddDbContext<ShippingContext>(options =>
            options.UseNpgsql(connectionString));

        // Repositories
        services.AddScoped<IShippingRepository, ShippingRepository>();
        services.AddScoped<IShippingMethodRepository, ShippingMethodRepository>();

        // Unit of Work
        services.AddScoped<IUnitOfWork, ShippingUnitOfWork>();

        return services;
    }

    public static async Task ApplyShippingMigrationsAsync(this IHost host)
    {
        using var scope = host.Services.CreateScope();

        var context = scope.ServiceProvider
            .GetRequiredService<ShippingContext>();

        await context.Database.MigrateAsync();
    }
}