

namespace Ecommerce.Order.Modules.Domain.Events;

internal sealed class OrderItemAddedDomainEvent(Guid OrderId,
                                                Guid OrderItemId,
                                                Guid ProductId,
                                                int Quantity) : DomainEvent
{
    public Guid OrderId { get; set; } = OrderId;
    public Guid OrderItemId { get; set; } = OrderItemId;
    public Guid ProductId { get; set; } = ProductId;
    public int Quantity { get; set; } = Quantity;
}
