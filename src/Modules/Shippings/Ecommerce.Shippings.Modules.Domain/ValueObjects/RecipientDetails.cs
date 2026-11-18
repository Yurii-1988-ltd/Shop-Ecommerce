using Ecommerce.Domain.Domain;
using Ecommerce.Domain.ValueObjects;

public sealed class RecipientDetails : ValueObject
{
    public string RecipientName { get; }
    public string RecipientPhone { get; }
    public string Country { get; }
    public string City { get; }
    public string? Street { get; }
    public string? Building { get; }
    public string? PostalCode { get; }
    public string? DeliveryPoint { get; }

    private RecipientDetails(
        string recipientName,
        string recipientPhone,
        string country,
        string city,
        string? street,
        string? building,
        string? postalCode,
        string? deliveryPoint)
    {
        RecipientName = recipientName;
        RecipientPhone = recipientPhone;
        Country = country;
        City = city;
        Street = street;
        Building = building;
        PostalCode = postalCode;
        DeliveryPoint = deliveryPoint;
    }

    public static Result<RecipientDetails> Create(
        string name,
        string phone,
        string country,
        string city,
        string? street,
        string? building,
        string? postalCode,
        string? deliveryPoint)
    {
       
        return new RecipientDetails(
            name.Trim(),
            phone.Trim(),
            country.Trim(),
            city.Trim(),
            street?.Trim(),
            building?.Trim(),
            postalCode?.Trim(),
            deliveryPoint?.Trim());
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return RecipientName;
        yield return RecipientPhone;
        yield return Country;
        yield return City;
        yield return Street ?? string.Empty;
        yield return Building ?? string.Empty;
        yield return PostalCode ?? string.Empty;
        yield return DeliveryPoint ?? string.Empty;
    }
}