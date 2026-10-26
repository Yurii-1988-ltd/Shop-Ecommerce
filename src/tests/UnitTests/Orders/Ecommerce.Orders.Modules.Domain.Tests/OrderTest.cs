
using Ecommerce.Domain.Constants;
using Ecommerce.Domain.ValueObjects;
using Ecommerce.Order.Modules.Domain.Entities;
using Ecommerce.Order.Modules.Domain.Enums;
using Ecommerce.Order.Modules.Domain.Errors;
using Ecommerce.Order.Modules.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace Ecommerce.Orders.Modules.Domain.Tests;

public sealed class OrderTests
{
    [Fact]
    public void Create_Should_Create_Order()
    {
        var customerId = Guid.NewGuid();
        const string customerEmail = "ivan@example.com";
        const string orderNumber = "ORD-2026-000001";

        var address = new OrderAddress(
            "Ivan",
            "Ivanov",
            "Ukraine",
            "Kyiv",
            "Khreshchatyk 1",
            "01001");

        var result = Ecommerce.Order.Modules.Domain.Entities.Order.Create(
            customerId,
            customerEmail,
            orderNumber,
            address);

        result.IsSuccess.Should().BeTrue();

        var order = result.Value;

        order.CustomerId.Should().Be(customerId);
        order.CustomerEmail.Should().Be(customerEmail);
        order.OrderNumber.Should().Be(orderNumber);
        order.Slug.Should().Be("ord-2026-000001");
        order.ShippingAddress.Should().Be(address);
        order.Currency.Should().Be(CurrencyConstant.UAH);
        order.Status.Should().Be(OrderStatus.Draft);
        order.Items.Should().BeEmpty();
    }

    [Fact]
    public void Create_Should_Return_Failure_When_CustomerId_Is_Empty()
    {
        var address = new OrderAddress(
            "Ivan",
            "Ivanov",
            "Ukraine",
            "Kyiv",
            "Khreshchatyk 1",
            "01001");

        var result = Ecommerce.Order.Modules.Domain.Entities.Order.Create(
            Guid.Empty,
            "ivan@example.com",
            "ORD-2026-000001",
            address);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OrderErrors.GuidEmpty);
    }

    [Fact]
    public void Create_Should_Create_Order_With_Draft_Status()
    {
        var customerId = Guid.NewGuid();
        const string customerEmail = "ivan@example.com";
        const string orderNumber = "ORD-2026-000001";

        var address = new OrderAddress(
            "Ivan",
            "Ivanov",
            "Ukraine",
            "Kyiv",
            "Khreshchatyk 1",
            "01001");

        var result = Ecommerce.Order.Modules.Domain.Entities.Order.Create(
            customerId,
            customerEmail,
            orderNumber,
            address);

        result.IsSuccess.Should().BeTrue();

        var order = result.Value;

        order.CustomerId.Should().Be(customerId);
        order.CustomerEmail.Should().Be(customerEmail);
        order.Status.Should().Be(OrderStatus.Draft);
        order.OrderNumber.Should().Be(orderNumber);
        order.Slug.Should().Be("ord-2026-000001");
        order.Items.Should().BeEmpty();
    }

    [Fact]
    public void ChangeItemQuantity_Should_Change_Quantity()
    {
        // Arrange
        var customerId = Guid.NewGuid();

        var address = new OrderAddress(
            "Ivan",
            "Ivanov",
            "Ukraine",
            "Kyiv",
            "Khreshchatyk 1",
            "01001");

        var order = Ecommerce.Order.Modules.Domain.Entities.Order.Create(
            customerId,
            "ivan@example.com",
            "ORD-2026-000001",
            address).Value;

        var originPrice = Money.Create(
            100,
            CurrencyConstant.UAH).Value;

        var salePrice = Money.Create(
            100,
            CurrencyConstant.UAH).Value;

        var productId = Guid.NewGuid();

        var addItemResult = order.AddItem(
            productId,
            "iPhone 16",
            "IP-001",
            originPrice,
            salePrice,
            1);

        addItemResult.IsSuccess.Should().BeTrue();

        var item = order.Items.First();

        // Act
        var result = order.ChangeItemQuantity(
            item.Id,
            5);

        // Assert
        result.IsSuccess.Should().BeTrue();
        item.Quantity.Should().Be(5);
        item.OriginUnitPrice.Amount.Should().Be(100);
        item.UnitPrice.Amount.Should().Be(100);
    }
}
