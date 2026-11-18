
using Ecommerce.Shippings.Modules.Domain.Entities;

namespace Ecommerce.Shippings.Modules.Domain.Repositories;

public interface IShippingMethodRepository
{
    Task<ShippingMethod?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<ShippingMethod?> GetByCodeAsync(
        string code,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<ShippingMethod>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<ShippingMethod>> GetActiveAsync(
        CancellationToken cancellationToken = default);

    Task AddAsync(
        ShippingMethod shippingMethod,
        CancellationToken cancellationToken = default);
    Task<bool> IsUsedAsync(Guid ShippingMethodId, CancellationToken cancellationToken = default);

    void Update(ShippingMethod shippingMethod);

    void Remove(ShippingMethod shippingMethod);
    Task<bool> ExistsByCodeAsync(
    string code,
    Guid? excludeId = null,
    CancellationToken cancellationToken = default);
}