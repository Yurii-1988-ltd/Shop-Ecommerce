


namespace Ecommerce.Shippings.Modules.Application.Features.UpdateTrackingNumber;

public sealed record UpdateTrackingNumberCommand(Guid ShippingId, string TrackingNumber)
                                                    : ICommand<Guid>;

