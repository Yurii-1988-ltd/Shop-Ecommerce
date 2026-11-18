namespace Ecommerce.Shippings.Modules.Application.Abstractions;



public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default);
}