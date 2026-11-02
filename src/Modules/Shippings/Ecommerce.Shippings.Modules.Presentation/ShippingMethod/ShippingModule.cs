

namespace Ecommerce.Shippings.Modules.Presentation.ShippingMethod;

public class ShippingModule : IModule
{
    public void RegisterServices(IServiceCollection services, IConfiguration config)
    {
        services.AddShippingModule(config);
    }

    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        new GetShippingMethodsEndpoint().MapEndpoints(app);
        new GetShippingMethodEndpoint().MapEndpoints(app);
        new CreateShippingMethodEndpoint().MapEndpoint(app);
        new UpdateShippingMethodEndpoint().MapEndpoint(app);
    }
}