

using Ecommerce.Shippings.Modules.Presentation.ShippingMethod;

namespace Ecommerce.Shippings.Modules.Presentation;

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
        new ActivateShippingMethodEndpoint().MapEndpoints(app);
        new DeactivateShippingMethodEndpoint().MapEndpoints(app);
        new DeleteShippingMethodsEndpoint().MapEndpoints(app);
        new CreateShippingEndpoint().MapEndpoints(app);
        new GetShippingByOrderIdEndpoint().MapEndpoints(app);
        new GetShippingsEndpoint().MapEndpoints(app);
    }
}