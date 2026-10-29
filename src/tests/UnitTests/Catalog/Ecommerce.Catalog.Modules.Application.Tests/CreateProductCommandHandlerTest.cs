using Ecommerce.Catalog.Modules.Application.Features.Products.CreateProduct;
using Ecommerce.Catalog.Modules.Domain.Repositories;
using FluentAssertions;
using Moq;
using Ecommerce.Catalog.Modules.Domain.Entities;
using Xunit;

namespace Ecommerce.Catalog.Modules.Application.Tests;

public class CreateProductCommandHandlerTest
{
    [Fact]
    public async Task Handle_Should_Create_Product_With_Category()
    {
        // Arrange
        var repository = new Mock<IProductRepository>();

        var handler = new CreateCatalogCommandHandler(repository.Object);

        // Тестовый ID категории
        var categoryId = Guid.NewGuid();

        var command = new CreateCatalogCommand(
            Name: "iPhone 16",
            ProductNumber: "PRD-2026-004588",
            Description: "Apple phone",
            Sku: "IPH16",
            Price: 1000m,
            Currency: "USD",
            CategoryId: categoryId 
        );

        Product? capturedProduct = null;

 
        repository
            .Setup(x => x.InsertAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .Callback<Product, CancellationToken>((product, _) => capturedProduct = product)
            .Returns(Task.CompletedTask);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();

        // Проверяем, что продукт действительно получил переданный CategoryId
        capturedProduct.Should().NotBeNull();
        capturedProduct!.CategoryId.Should().Be(categoryId);

        repository.Verify(
            x => x.InsertAsync(
                It.IsAny<Product>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}