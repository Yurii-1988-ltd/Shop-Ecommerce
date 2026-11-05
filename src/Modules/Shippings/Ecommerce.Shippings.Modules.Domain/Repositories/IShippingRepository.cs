using Ecommerce.Shipings.Modules.Domain.Entities;
using Ecommerce.Shippings.Modules.Domain.Entities;


namespace Ecommerce.Shippings.Modules.Domain.Repositories;

public interface IShippingRepository
{
    Task<Shipping?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<Shipping?> GetByOrderIdAsync(
        Guid orderId,
        CancellationToken cancellationToken = default);

    Task<Shipping?> GetByTrackingNumberAsync(
        string trackingNumber,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Shipping>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Shipping shipping,
        CancellationToken cancellationToken = default);

    void Update(Shipping shipping);

    void Remove(Shipping shipping);
}