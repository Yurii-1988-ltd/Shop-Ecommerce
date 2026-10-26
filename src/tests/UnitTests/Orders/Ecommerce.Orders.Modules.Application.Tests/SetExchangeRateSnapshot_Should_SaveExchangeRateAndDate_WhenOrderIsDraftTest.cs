
namespace Ecommerce.Orders.Modules.Application.Tests;

public class SetExchangeRateSnapshot_Should_SaveExchangeRateAndDate_WhenOrderIsDraftTest
{
    [Fact]
    public void SetExchangeRateSnapshot_Should_SaveExchangeRateAndDate_WhenOrderIsDraft()
    {
        // Arrange
        var orderResult = Order.Modules.Domain.Entities.Order.Create(
            customerId: Guid.NewGuid(),
            customerEmail: "test@example.com",
            orderNumber: "ORD-2026-100004",
            shippingAddress: new OrderAddress(
                "Ivan", "Ivanov", "Ukraine",
                "Kyiv", "Khreshchatyk 1", "01001"),
            currency: "USD");

        orderResult.IsSuccess.Should().BeTrue();

        var order = orderResult.Value;
        var exchangeRateDate = new DateOnly(2026, 10, 26);

        // Act
        var result = order.SetExchangeRateSnapshot(40m, exchangeRateDate);

        // Assert
        result.IsSuccess.Should().BeTrue();
        order.ExchangeRate.Should().Be(40m);
        order.ExchangeRateDate.Should().Be(exchangeRateDate);
        order.ExchangeRateCapturedAtUtc.Should().NotBeNull();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void SetExchangeRateSnapshot_Should_ReturnFailure_WhenExchangeRateIsNotPositive(
        decimal exchangeRate)
    {
        // Arrange
        var orderResult = Order.Modules.Domain.Entities.Order.Create(
            customerId: Guid.NewGuid(),
            customerEmail: "test@example.com",
            orderNumber: "ORD-2026-100005",
            shippingAddress: new OrderAddress(
                "Ivan", "Ivanov", "Ukraine",
                "Kyiv", "Khreshchatyk 1", "01001"),
            currency: "USD");

        orderResult.IsSuccess.Should().BeTrue();

        var order = orderResult.Value;

        // Act
        var result = order.SetExchangeRateSnapshot(
            exchangeRate,
            new DateOnly(2026, 10, 26));

        // Assert
        result.IsFailure.Should().BeTrue();
        order.ExchangeRate.Should().BeNull();
        order.ExchangeRateDate.Should().BeNull();
        order.ExchangeRateCapturedAtUtc.Should().BeNull();
    }

    [Fact]
    public void SetExchangeRateSnapshot_Should_ReturnFailure_WhenOrderIsNotDraft()
    {
        // Arrange
        var orderResult = Order.Modules.Domain.Entities.Order.Create(
            customerId: Guid.NewGuid(),
            customerEmail: "test@example.com",
            orderNumber: "ORD-2026-100006",
            shippingAddress: new OrderAddress(
                "Ivan", "Ivanov", "Ukraine",
                "Kyiv", "Khreshchatyk 1", "01001"),
            currency: "USD");

        orderResult.IsSuccess.Should().BeTrue();

        var order = orderResult.Value;

        order.SetExchangeRateSnapshot(
            40m,
            new DateOnly(2026, 10, 26));

        var originPrice = Money.Create(150m, CurrencyConstant.UAH).Value;
        var unitPrice = Money.Create(3.75m, "USD").Value;

        var addItemResult = order.AddItem(
            Guid.NewGuid(),
            "Test Product",
            "TST-001",
            originPrice,
            unitPrice,
            quantity: 2);

        addItemResult.IsSuccess.Should().BeTrue();

        var submitResult = order.Submit();

        submitResult.IsSuccess.Should().BeTrue(
            $"заказ должен перейти в Pending: {submitResult.Error}");

        var originalRate = order.ExchangeRate;
        var originalDate = order.ExchangeRateDate;
        var originalCapturedAt = order.ExchangeRateCapturedAtUtc;

        // Act
        var result = order.SetExchangeRateSnapshot(
            42m,
            new DateOnly(2026, 10, 27));

        // Assert
        result.IsFailure.Should().BeTrue();
        order.ExchangeRate.Should().Be(originalRate);
        order.ExchangeRateDate.Should().Be(originalDate);
        order.ExchangeRateCapturedAtUtc.Should().Be(originalCapturedAt);
    }
  
        [Fact]
        public void GetTotalAmount_Should_ReturnTotalInOrderCurrency()
        {
            // Arrange
            var orderResult = Order.Modules.Domain.Entities.Order.Create(
                customerId: Guid.NewGuid(),
                customerEmail: "test@example.com",
                orderNumber: "ORD-2026-100005",
                shippingAddress: new OrderAddress(
                    "Ivan",
                    "Ivanov",
                    "Ukraine",
                    "Kyiv",
                    "Khreshchatyk 1",
                    "01001"),
                currency: "USD");

            orderResult.IsSuccess.Should().BeTrue();
            var order = orderResult.Value;

            var originPrice1 = Money.Create(150m, CurrencyConstant.UAH).Value;
            var unitPrice1 = Money.Create(3.75m, "USD").Value;

            var originPrice2 = Money.Create(200m, CurrencyConstant.UAH).Value;
            var unitPrice2 = Money.Create(5.25m, "USD").Value;

            var firstItemResult = order.AddItem(
                Guid.NewGuid(),
                "Product 1",
                "SKU-001",
                originPrice1,
                unitPrice1,
                quantity: 2);

            var secondItemResult = order.AddItem(
                Guid.NewGuid(),
                "Product 2",
                "SKU-002",
                originPrice2,
                unitPrice2,
                quantity: 1);

            firstItemResult.IsSuccess.Should().BeTrue();
            secondItemResult.IsSuccess.Should().BeTrue();

            // Act
            var totalResult = order.GetTotalAmount();

            // Assert
            totalResult.IsSuccess.Should().BeTrue();
            totalResult.Value.Amount.Should().Be(12.75m);
            totalResult.Value.Currency.Should().Be("USD");
        }
    }

