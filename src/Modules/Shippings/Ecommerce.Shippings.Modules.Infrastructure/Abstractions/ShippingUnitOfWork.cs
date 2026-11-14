

namespace Ecommerce.Shippings.Modules.Infrastructure.Abstractions;

internal sealed class ShippingUnitOfWork(ShippingContext context) : IUnitOfWork
{
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    => await context.SaveChangesAsync(cancellationToken);
}