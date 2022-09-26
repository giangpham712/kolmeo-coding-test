using KolmeoCodingTest.Core.Domain.Entities;
using KolmeoCodingTest.Core.Domain.Errors;
using KolmeoCodingTest.Core.Domain.Repositories;

namespace KolmeoCodingTest.Core.Domain.Services;

public class ProductManager : IProductManager
{
    private readonly IProductsRepository _productsRepository;

    public ProductManager(IProductsRepository productsRepository)
    {
        _productsRepository = productsRepository;
    }

    public async Task<Product> CreateProduct(string name, string description, float price)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException(ErrorType.Validation, "Product name must not be empty.");
        }
        
        if (price < 0)
        {
            throw new DomainException(ErrorType.Validation, "Product price must not be negative.");
        }

        var product = new Product()
        {
            Name = name,
            Description = description,
            Price = price
        };
        
        await _productsRepository.Create(product);
        return product;
    }

    public async Task<Product> UpdateProduct(Product product, string name, string description, float price)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException(ErrorType.Validation, "Product name must not be empty.");
        }
        
        if (price < 0)
        {
            throw new DomainException(ErrorType.Validation, "Product price must not be negative.");
        }
        
        product.Name = name;
        product.Description = description;
        product.Price = price;

        await _productsRepository.Update(product);
        return product;
    }
}