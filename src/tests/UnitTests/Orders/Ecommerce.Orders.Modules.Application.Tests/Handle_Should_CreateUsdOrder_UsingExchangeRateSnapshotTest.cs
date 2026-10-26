

namespace Ecommerce.Orders.Modules.Application.Tests;

public class Handle_Should_CreateUsdOrder_UsingExchangeRateSnapshotTest
{
    [Fact]
    public async Task Handle_Should_CreateUsdOrder_UsingExchangeRateSnapshot()
    {
        // Arrange
        var repositoryMock = new Mock<IOrderRepository>();
        var orderNumberGeneratorMock = new Mock<IEntityNumberGenerator>();
        var conversionServiceMock = new Mock<ICurrencyConversionService>();
        var publishEndpointMock = new Mock<IPublishEndpoint>();

        const string orderNumber = "ORD-2026-100002";
        const string customerEmail = "test@example.com";
        const decimal exchangeRate = 40m;

        var customerId = Guid.NewGuid();
        var exchangeRateDate = new DateOnly(2026, 10, 26);
        var capturedAtUtc = DateTime.UtcNow;

        orderNumberGeneratorMock
            .Setup(x => x.GenerateAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(orderNumber);

        repositoryMock
            .Setup(x => x.InsertAsync(
                It.IsAny<Order.Modules.Domain.Entities.Order>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        conversionServiceMock
            .Setup(x => x.ConvertWithRateAsync(
                It.Is<Money>(m =>
                    m.Amount == 150m &&
                    m.Currency == CurrencyConstant.UAH),
                "USD",
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(
                new CurrencyConversionResult(
                    Money.Create(3.75m, "USD").Value,
                    exchangeRate,
                    exchangeRateDate,
                    capturedAtUtc)));

        conversionServiceMock
            .Setup(x => x.ConvertAtRateAsync(
                It.IsAny<Money>(),
                "USD",
                exchangeRate,
                It.IsAny<CancellationToken>()))
            .Returns<Money, string, decimal, CancellationToken>(
                (source, targetCurrency, rate, cancellationToken) =>
                    Task.FromResult(
                        Result.Success(
                            Money.Create(
                                Math.Round(
                                    source.Amount / rate,
                                    2,
                                    MidpointRounding.AwayFromZero),
                                targetCurrency).Value)));

        var handler = new CreateOrderCommandHandler(
            repositoryMock.Object,
            orderNumberGeneratorMock.Object,
            conversionServiceMock.Object,
            publishEndpointMock.Object);

        var addressDto = new OrderAddressDto(
            "Ivan",
            "Ivanov",
            "Ukraine",
            "Kyiv",
            "Khreshchatyk 1",
            "01001");

        var itemsDto = new List<CreateOrderItemDto>
    {
        new(
            ProductId: Guid.NewGuid(),
            ProductName: "Test Product",
            Sku: "TST-001",
            UnitPrice: 150m,
            Quantity: 2)
    };

        var command = new CreateOrderCommand(
            customerId,
            customerEmail,
            addressDto,
            itemsDto,
            "USD");

        // Act
        var result = await handler.Handle(
            command,
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        repositoryMock.Verify(
            x => x.InsertAsync(
                It.Is<Order.Modules.Domain.Entities.Order>(o =>
                    o.Currency == "USD" &&
                    o.CustomerId == customerId &&
                    o.ExchangeRate == exchangeRate &&
                    o.ExchangeRateDate == exchangeRateDate &&
                    o.Items.Count == 1 &&
                    o.Items.First().OriginUnitPrice.Amount == 150m &&
                    o.Items.First().UnitPrice.Amount == 3.75m &&
                    o.GetTotalAmount().Value.Amount == 7.50m),
                It.IsAny<CancellationToken>()),
            Times.Once);

        publishEndpointMock.Verify(
            x => x.Publish(
                It.Is<OrderCreatedIntegrationEvent>(e =>
                    e.Currency == "USD" &&
                    e.TotalAmount == 7.50m &&
                    e.CustomerId == customerId &&
                    e.CustomerEmail == customerEmail),
                It.IsAny<CancellationToken>()),
            Times.Once);

        conversionServiceMock.Verify(
            x => x.ConvertWithRateAsync(
                It.IsAny<Money>(),
                "USD",
                It.IsAny<CancellationToken>()),
            Times.Once);

        conversionServiceMock.Verify(
            x => x.ConvertAtRateAsync(
                It.IsAny<Money>(),
                "USD",
                exchangeRate,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
