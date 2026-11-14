using Ecommerce.Presentation;
using Ecommerce.Shippings.Modules.Presentation.ShippingMethod;


namespace Ecommerce.Shippings.Modules.Presentation;

public class ShippingModule : IModule
{
    public void RegisterServices(
        IServiceCollection services,
        IConfiguration config)
    {
        services.AddShippingModule(config);
    }

    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        // Shipping Methods
        new GetShippingMethodsEndpoint().MapEndpoints(app);
        new GetShippingMethodByIdEndpoint().MapEndpoints(app);
        new CreateShippingMethodEndpoint().MapEndpoints(app);
        new UpdateShippingMethodEndpoint().MapEndpoints(app);
        new DeleteShippingMethodEndpoint().MapEndpoints(app);
        new ActivateShippingMethodEndpoint().MapEndpoints(app);
        new DeactivateShippingMethodEndpoint().MapEndpoints(app);

        // Shippings
        new CreateShippingEndpoint().MapEndpoints(app);
        new GetShippingByOrderIdEndpoint().MapEndpoints(app);
        new GetShippingsEndpoint().MapEndpoints(app);
        new UpdateTrackingNumberEndpoint().MapEndpoints(app);
        new CancelShippingEndpoint().MapEndpoints(app);
        new UpdateRecipientDetailsEndpoint().MapEndpoints(app);
        new GetShipmentTrackingEventsEndpoint().MapEndpoints(app);
        new StartProcessingEndpoint().MapEndpoints(app);
        new MarkAsReadyForShipmentEndpoint().MapEndpoints(app);
        new MarkAsShippedEndpoint().MapEndpoints(app);
        new MarkAsDeliveredEndpoint().MapEndpoints(app);
        new MarkInTransitEndpoint().MapEndpoints(app);
        new MarkOutForDeliveryEndpoint().MapEndpoints(app);
        new MarkAsFailedEndpoint().MapEndpoints(app);
        new MarkAsReturnedEndpoint().MapEndpoints(app);
        new GetShippingByIdEndpoint().MapEndpoints(app);
     
    }

}