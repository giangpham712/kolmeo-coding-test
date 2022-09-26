using KolmeoCodingTest.Core.Domain.Entities;

namespace KolmeoCodingTest.Core.Domain.Services;

public interface IProductManager
{
    Task<Product> CreateProduct(string name, string description, float price);
    Task<Product> UpdateProduct(Product product, string name, string description, float price);
}