

namespace Ecommerce.Orders.Modules.Application.Tests;

public class Handle_Should_ReturnFailure_WhenCurrencyConversionFailsTest
{
    [Fact]
    public async Task Handle_Should_ReturnFailure_WhenCurrencyConversionFails()
    {
        // Arrange
        var repositoryMock = new Mock<IOrderRepository>();
        var orderNumberGeneratorMock = new Mock<IEntityNumberGenerator>();
        var conversionServiceMock = new Mock<ICurrencyConversionService>();
        var publishEndpointMock = new Mock<IPublishEndpoint>();

        orderNumberGeneratorMock
            .Setup(x => x.GenerateAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync("ORD-2026-100003");

        conversionServiceMock
            .Setup(x => x.ConvertWithRateAsync(
                It.IsAny<Money>(),
                "USD",
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                Result.Failure<CurrencyConversionResult>(
                    Error.Failure(
                        "Currency.ExchangeRateUnavailable",
                        "Exchange rate is unavailable.")));

        var handler = new CreateOrderCommandHandler(
            repositoryMock.Object,
            orderNumberGeneratorMock.Object,
            conversionServiceMock.Object,
            publishEndpointMock.Object);

        var addressDto = new OrderAddressDto(
            "Ivan", "Ivanov", "Ukraine", "Kyiv",
            "Khreshchatyk 1", "01001");

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
            Guid.NewGuid(),
            "test@example.com",
            addressDto,
            itemsDto,
            "USD");

        // Act
        var result = await handler.Handle(
            command,
            CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();

        repositoryMock.Verify(
            x => x.InsertAsync(
                It.IsAny<Order.Modules.Domain.Entities.Order>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        publishEndpointMock.Verify(
            x => x.Publish(
                It.IsAny<OrderCreatedIntegrationEvent>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
