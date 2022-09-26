using KolmeoCodingTest.Core.Domain.Entities;
using KolmeoCodingTest.Core.Domain.Errors;
using KolmeoCodingTest.Core.Domain.Repositories;
using KolmeoCodingTest.Core.Domain.Services;
using Moq;

namespace KolmeoCodingTest.Core.Tests;

public class ProductManagerTests
{
    [Fact]
    public async Task CreateProduct_ValidData_ShouldCreateProduct()
    {
        var product = new Product()
        {
            Id = 1,
            Name = "Test product 1",
            Description = "Description for Test product 1",
            Price = 10
        };
        
        var productsRepository = new Mock<IProductsRepository>();
        productsRepository
            .Setup(x => x.Create(It.IsAny<Product>()))
            .Callback((Product p) => p.Id = 1);
        
        var productManager = new ProductManager(productsRepository.Object);

        var createdProduct = await productManager.CreateProduct("Test product 1", "Description for Test product 1", 10);
        
        Assert.NotNull(createdProduct);
        
        Assert.Equal(product.Id, createdProduct.Id);
        Assert.Equal(product.Name, createdProduct.Name);
        Assert.Equal(product.Description, createdProduct.Description);
        Assert.Equal(product.Price, createdProduct.Price);
    }

    [Fact]
    public async Task CreateProduct_NegativePrice_ShouldThrowException()
    {
        var productsRepository = new Mock<IProductsRepository>();
        
        var productManager = new ProductManager(productsRepository.Object);
        var exception = await Assert.ThrowsAsync<DomainException>(async () =>
            await productManager.CreateProduct("Test product 1", "Description for Test product 1", -100));
        
        Assert.Equal(ErrorType.Validation, exception.Type);
        Assert.Equal("Product price must not be negative.", exception.Message);
    }
    
    [Fact]
    public async Task CreateProduct_EmptyName_ShouldThrowException()
    {
        var productsRepository = new Mock<IProductsRepository>();
        
        var productManager = new ProductManager(productsRepository.Object);
        var exception = await Assert.ThrowsAsync<DomainException>(async () =>
            await productManager.CreateProduct("", "Description for Test product 1", 100));
        
        Assert.Equal(ErrorType.Validation, exception.Type);
        Assert.Equal("Product name must not be empty.", exception.Message);
    }
}