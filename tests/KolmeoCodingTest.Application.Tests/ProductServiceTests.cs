using KolmeoCodingTest.Application.Errors;
using KolmeoCodingTest.Core.Domain.Entities;
using KolmeoCodingTest.Core.Domain.Repositories;
using KolmeoCodingTest.Core.Domain.Services;
using Moq;

namespace KolmeoCodingTest.Application.Tests;

public class ProductServiceTests
{
    [Fact]
    public async Task GetProduct_InvalidProduct_ShouldThrowException()
    {
        var productsRepository = new Mock<IProductsRepository>();
        productsRepository.Setup(x => x.Get(It.IsAny<int>())).ReturnsAsync(null as Product);

        var productManager = new Mock<IProductManager>();
        var productsService = new ProductsService(productsRepository.Object, productManager.Object);

        var exception = await Assert.ThrowsAsync<EntityNotFoundException>(async () => await productsService.GetProduct(1));
        
        Assert.NotNull(exception);
        Assert.Equal("Product with ID 1 could not be found.", exception.Message);
        Assert.Equal(nameof(Product), exception.EntityName);
        Assert.Equal(1, exception.EntityId);
    }
}