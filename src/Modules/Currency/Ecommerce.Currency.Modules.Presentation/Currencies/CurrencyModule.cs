

using Ecommerce.Currency.Modules.Infrastructure;
using Ecommerce.Presentation;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ecommerce.Currency.Modules.Presentation.Currencies;

public sealed class CurrencyModule : IModule
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        new GetLatestExchangeRatesEndpoint().MapEndPoint(app);
        new SynchronizeExchangeRatesEndpoint().MapEndPoint(app);
        new ConvertCurrencyEndpoint().MapEndPoint(app);
    }

    public void RegisterServices(IServiceCollection services, IConfiguration config)
    {
        services.AddCurrencyModule(config);
    }
}
