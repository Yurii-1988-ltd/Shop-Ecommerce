namespace Ecommerce.Shared.Contracts.IntegrationEvent;

public sealed record OrderPaidIntegrationEvent(
    Guid OrderId,
    string OrderNumber,
    Guid? CustomerId,
    string CustomerEmail,
    OrderAddressIntegrationDto ShippingAddress,
    Guid ShippingMethodId,
    string ShippingMethodName,
    decimal ShippingCostAmount,
    string ShippingCostCurrency,
    string RecipientPhone,
    DateTime PaidAtUtc
);

public sealed record OrderAddressIntegrationDto(
    string FirstName,
    string LastName,
    string Country,
    string City,
    string Street,
    string ZipCode
);