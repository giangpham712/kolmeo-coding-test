using KolmeoCodingTest.Application.Contracts.DTOs;

namespace KolmeoCodingTest.Application.Contracts.Interfaces;

public interface IProductsService
{
    Task<ProductDTO> CreateProduct(CreateOrUpdateProductDTO dto);
    Task<IEnumerable<ProductDTO>> ListProducts();
    Task<ProductDTO> GetProduct(int id);
    Task<ProductDTO> UpdateProduct(int id, CreateOrUpdateProductDTO dto);
    Task DeleteProduct(int id);
}