
namespace Ecommerce.Application.Abstractions
{
    public interface IEntityNumberGenerator
    {
        Task<string> GenerateAsync(
        string prefix,
        CancellationToken cancellationToken = default);
     
    }
}
