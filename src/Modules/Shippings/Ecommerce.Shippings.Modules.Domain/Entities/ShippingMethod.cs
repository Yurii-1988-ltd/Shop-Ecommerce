
using Ecommerce.Domain.Domain;
using Ecommerce.Domain.ValueObjects;
using Ecommerce.Shippings.Modules.Domain.Errors;

namespace Ecommerce.Shippings.Modules.Domain.Entities;

public sealed class ShippingMethod : Entity
{
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public Money Price { get; private set; } = Money.Zero();
    public bool IsFree => Price.Amount == 0;

    public bool IsActive { get; private set; }

    private ShippingMethod()
    {
    }

    private ShippingMethod(
        Guid id,
        string name,
        string? description,
        Money price)
    {
        Id = id;
        Name = name;
        Description = description;
        Price = price;
        IsActive = true;
    }

    public static Result<ShippingMethod> Create(
        Guid id,
        string name,
        string? description,
        Money price)
    {
        if (id == Guid.Empty)
            return ShippingErrors.GuidEmpty;

        if (string.IsNullOrWhiteSpace(name))
            return ShippingErrors.NameIsRequired;
        if (price is null)
            return ShippingErrors.InvalidPrice;

        return new ShippingMethod(
            id,
            name.Trim(),
            description?.Trim(),
            price);
    }

    public Result Update(
        string name,
        string? description,
        Money price)
    {
        if (string.IsNullOrWhiteSpace(name))
            return ShippingErrors.NameIsRequired;

        if (price is null)
            return ShippingErrors.InvalidPrice;

        Name = name.Trim();
        Description = description?.Trim();
        Price = price;

        return Result.Success();
    }

    public Result Activate()
    {
        if (IsActive)
            return ShippingErrors.ShippingMethodAlreadyActive;

        IsActive = true;

        return Result.Success();
    }

    public Result Deactivate()
    {
        if (!IsActive)
            return ShippingErrors.ShippingMethodAlreadyInactive;

        IsActive = false;

        return Result.Success();
    }
}
