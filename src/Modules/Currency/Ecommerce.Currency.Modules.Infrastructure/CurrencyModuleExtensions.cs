

using Ecommerce.Currency.Modules.Application.Features.GetLatestExchangeRates;

namespace Ecommerce.Currency.Modules.Infrastructure;

public static class CurrencyModuleExtensions
{
    public static IServiceCollection AddCurrencyModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddApplication()
            .AddInfrastructure(configuration);

        return services;
    }

    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(typeof(GetLatestExchangeRatesQuery).Assembly);
        });

        return services;
    }

    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString =
            configuration.GetConnectionString("currencies")
            ?? throw new InvalidOperationException(
                "Connection string 'currencies' was not found.");

        services.AddNpgsqlDataSource(
            connectionString,
            serviceKey: "currency");

        services.AddDbContext<ExchangeRateContext>((serviceProvider, options) =>
        {
            var dataSource =
                serviceProvider.GetRequiredKeyedService<NpgsqlDataSource>(
                    "currency");

            options.UseNpgsql(dataSource);
        });

        services
            .AddOptions<NbuOptions>()
            .Bind(configuration.GetSection(NbuOptions.SectionName))
            .Validate(
                options =>
                    Uri.TryCreate(
                        options.BaseUrl,
                        UriKind.Absolute,
                        out var uri) &&
                    uri.Scheme == Uri.UriSchemeHttps,
                "A valid HTTPS NBU BaseUrl is required.")
            .ValidateOnStart();

        services.AddHttpClient<
            IExchangeRateProvider,
            NbuExchangeRateProvider>();


        services.AddScoped<IExchangeRateRepository, ExchangeRateRepository>();    
        services.AddScoped<ICurrencyConversionService, CurrencyConversionService>();
        services.AddHostedService<ExchangeRateSynchronizationBackgroundService>();

        return services;
    }

    public static async Task MigrateDatabaseAsync(
        this WebApplication app)
    {
        await using var scope = app.Services.CreateAsyncScope();

        var dbContext = scope.ServiceProvider
            .GetRequiredService<ExchangeRateContext>();

        await dbContext.Database.MigrateAsync();
    }
}