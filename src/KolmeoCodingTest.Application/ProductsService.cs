using KolmeoCodingTest.Application.Contracts.DTOs;
using KolmeoCodingTest.Application.Contracts.Interfaces;
using KolmeoCodingTest.Application.Errors;
using KolmeoCodingTest.Core.Domain.Entities;
using KolmeoCodingTest.Core.Domain.Repositories;
using KolmeoCodingTest.Core.Domain.Services;

namespace KolmeoCodingTest.Application;

public class ProductsService : IProductsService
{
    private readonly IProductsRepository _productsRepository;
    private readonly IProductManager _productManager;

    public ProductsService(IProductsRepository productsRepository, IProductManager productManager)
    {
        _productsRepository = productsRepository;
        _productManager = productManager;
    }

    public async Task<ProductDTO> CreateProduct(CreateOrUpdateProductDTO dto)
    {
        var product = await _productManager.CreateProduct(
            dto.Name, 
            dto.Description, 
            dto.Price);
        
        return product.ToDTO();
    }

    public async Task<IEnumerable<ProductDTO>> ListProducts()
    {
        var products = await _productsRepository.List();
        return products.Select(x => x.ToDTO());
    }

    public async Task<ProductDTO> GetProduct(int id)
    {
        var product = await GetProductOrThrow(id);
        return product.ToDTO();
    }

    public async Task<ProductDTO> UpdateProduct(int id, CreateOrUpdateProductDTO dto)
    {
        var product = await GetProductOrThrow(id);
        var updatedProduct = await _productManager.UpdateProduct(product, dto.Name, dto.Description, dto.Price);
        return updatedProduct.ToDTO();
    }

    public async Task DeleteProduct(int id)
    {
        var product = await GetProductOrThrow(id);

        await _productsRepository.Delete(product);
    }

    private async Task<Product> GetProductOrThrow(int id)
    {
        var product = await _productsRepository.Get(id);
        if (product == null)
        {
            throw new EntityNotFoundException(nameof(Product), id);
        }

        return product;
    }
}