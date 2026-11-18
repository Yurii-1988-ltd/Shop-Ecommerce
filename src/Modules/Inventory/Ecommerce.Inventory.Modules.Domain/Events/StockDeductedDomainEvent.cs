using Ecommerce.Domain.Domain;

namespace Ecommerce.Inventory.Modules.Domain.Events;

public sealed class StockDeductedDomainEvent(
    Guid ProductId,
    int Quantity) : DomainEvent
{
    public Guid ProductId { get; } = ProductId;
    public int Quantity { get; } = Quantity;
}