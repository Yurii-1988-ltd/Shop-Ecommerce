
using Ecommerce.Catalog.Modules.Domain.Errors;
using Ecommerce.Catalog.Modules.Domain.Events;
using Ecommerce.Domain.Domain;
using Ecommerce.Domain.ValueObjects;
using MongoDB.Bson.Serialization.Attributes;

namespace Ecommerce.Catalog.Modules.Domain.Entities;

public sealed class Product : Entity
{
    [BsonElement("ProductName")]
    public string Name { get; private set; } = string.Empty;

    public string ProductNumber { get; private set; } = string.Empty;

    public string Description { get; private set; } = string.Empty;

    public string Sku { get; private set; } = string.Empty;

    public string Slug { get; private set; } = string.Empty;

    public Money Price { get; private set; } = default!;

    public Money? SalePrice { get; private set; }

    public int StockQuantity { get; private set; }

    [BsonIgnore]
    public bool InStock => StockQuantity > 0;

    [BsonIgnore]
    public bool OutOfStock => StockQuantity == 0;

    public bool IsActive { get; private set; }

    public Guid? CategoryId { get; private set; }

    public Guid? BrandId { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public DateTime? UpdatedAtUtc { get; private set; }

    public List<ProductImage> Images { get; private set; } = new();

    public List<ProductAttributeValue> AttributeValues { get; private set; } = new();

    private Product()
    {
    }

    public static Result<Product> Create(
        string name,
        string productNumber,
        string sku,
        Money price,
        string description = "")
    {
        if (string.IsNullOrWhiteSpace(name))
            return ProductErrors.NameIsRequired;

        if (string.IsNullOrWhiteSpace(sku))
            return ProductErrors.SkuIsRequired;

        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = name.Trim(),
            ProductNumber = productNumber,
            Sku = sku.Trim().ToUpperInvariant(),
            Price = price,
            Description = description,
            StockQuantity = 0,
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow
        };

        product.AddDomainEvent(new ProductCreatedDomainEvent(product.Id));

        return product;
    }

    public Result UpdatePrice(Money newPrice, Money? newSalePrice = null)
    {
        if (newSalePrice is not null)
        {
            if (newSalePrice.Currency != newPrice.Currency)
                return ProductErrors.CurrencyMismatch;

            if (newSalePrice.Amount > newPrice.Amount)
                return ProductErrors.InvalidSalePrice;
        }

        Price = newPrice;
        SalePrice = newSalePrice;
        UpdatedAtUtc = DateTime.UtcNow;

        return Result.Success();
    }

    public Result UpdateStock(int quantity)
    {
        if (quantity < 0)
            return ProductErrors.NegativeStockQuantity;

        StockQuantity = quantity;
        UpdatedAtUtc = DateTime.UtcNow;

        return Result.Success();
    }

    public Result UpdateInformation(
        string name,
        string description,
        string sku)
    {
        if (string.IsNullOrWhiteSpace(name))
            return ProductErrors.NameIsRequired;

        if (string.IsNullOrWhiteSpace(sku))
            return ProductErrors.SkuIsRequired;

        Name = name.Trim();
        Description = description;
        Sku = sku.Trim().ToUpperInvariant();

        UpdatedAtUtc = DateTime.UtcNow;

        return Result.Success();
    }

    public Result Archive()
    {
        if (!IsActive)
            return ProductErrors.ProductAlreadyArchived;

        IsActive = false;
        UpdatedAtUtc = DateTime.UtcNow;

        return Result.Success();
    }

    public Result AddImage(
        string storageKey,
        string? altText,
        Guid? productVariantId = null)
    {
        var imageResult = ProductImage.Create(
            productId: Id,
            storageKey: storageKey,
            altText: altText,
            isPrimary: Images.Count == 0,
            displayOrder: Images.Count,
            productVariantId: productVariantId);

        if (imageResult.IsFailure)
            return imageResult.Error;

        Images.Add(imageResult.Value);
        UpdatedAtUtc = DateTime.UtcNow;

        return Result.Success();
    }

    public Result SetPrimaryImage(Guid imageId)
    {
        var image = Images.FirstOrDefault(x => x.Id == imageId);

        if (image is null)
            return ImagesErrors.NotFound(imageId);

        if (image.IsPrimary)
            return Result.Success();

        foreach (var item in Images)
            item.RemovePrimary();

        image.SetAsPrimary();
        UpdatedAtUtc = DateTime.UtcNow;

        return Result.Success();
    }

    public Result RemoveImage(Guid imageId)
    {
        var image = Images.FirstOrDefault(x => x.Id == imageId);

        if (image is null)
            return ImagesErrors.NotFound(imageId);

        var wasPrimary = image.IsPrimary;

        Images.Remove(image);

        if (wasPrimary && Images.Count > 0)
        {
            Images
                .OrderBy(x => x.DisplayOrder)
                .First()
                .SetAsPrimary();
        }

        UpdatedAtUtc = DateTime.UtcNow;

        return Result.Success();
    }

    public Result ReorderImages(Guid imageId, int newIndex)
    {
        var image = Images.FirstOrDefault(x => x.Id == imageId);

        if (image is null)
            return ImagesErrors.NotFound(imageId);

        var ordered = Images
            .OrderBy(x => x.DisplayOrder)
            .ToList();

        ordered.Remove(image);

        newIndex = Math.Clamp(newIndex, 0, ordered.Count);

        ordered.Insert(newIndex, image);

        for (var i = 0; i < ordered.Count; i++)
            ordered[i].ChangeDisplayOrder(i);

        UpdatedAtUtc = DateTime.UtcNow;

        return Result.Success();
    }
}
