using Ecommerce.Domain.Domain;


namespace Ecommerce.Modules.Users.Domain.Events;

internal sealed class UserUpdatedDomainEvent(Guid UserId) : DomainEvent
{
    public Guid UserId { get; set; } = UserId;
}
