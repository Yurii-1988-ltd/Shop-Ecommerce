
using Ecommerce.Order.Modules.Application.Features.Responses;

namespace Ecommerce.Order.Modules.Application.Features.GetBySlug
{
    public sealed record GetBySlugQuery(string slug) : IQuery<OrderResponse>;
   
}
