
using Ecommerce.Shippings.Modules.Domain.Entities;
using Ecommerce.Shippings.Modules.Domain.Repositories;
using Ecommerce.Shippings.Modules.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Shippings.Modules.Infrastructure.Repositories;

public sealed class ShippingMethodRepository(ShippingContext context) : IShippingMethodRepository
{
    public async Task<ShippingMethod?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.ShippingMethods
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<ShippingMethod?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            return null;
        }

        return await context.ShippingMethods
            .FirstOrDefaultAsync(x => x.Code == code.Trim(), cancellationToken);
    }

    public async Task<IReadOnlyCollection<ShippingMethod>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await context.ShippingMethods
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<ShippingMethod>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        return await context.ShippingMethods
            .AsNoTracking()
            .Where(x => x.IsActive)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(ShippingMethod shippingMethod, CancellationToken cancellationToken = default)
    {
        await context.ShippingMethods.AddAsync(shippingMethod, cancellationToken);
    }

    public void Update(ShippingMethod shippingMethod)
    {
        context.ShippingMethods.Update(shippingMethod);
    }

    public void Remove(ShippingMethod shippingMethod)
    {
        context.ShippingMethods.Remove(shippingMethod);
    }
}