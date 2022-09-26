using KolmeoCodingTest.Core.Domain.Entities;

namespace KolmeoCodingTest.Application.Contracts.DTOs;

public static class MappingExtensions
{
    public static ProductDTO ToDTO(this Product product)
    {
        if (product == null)
        {
            throw new ArgumentNullException();
        }
        
        return new ProductDTO()
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price
        };
    }
}