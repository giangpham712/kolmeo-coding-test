using KolmeoCodingTest.Application.Contracts.DTOs;
using KolmeoCodingTest.Core.Domain.Entities;

namespace KolmeoCodingTest.Application.Tests;

public class UnitTest1
{
    [Fact]
    public void ToDTO_ValidProduct_ShouldMapCorrectly()
    {
        var product = new Product()
        {
            Id = 1,
            Name = "Product 1",
            Description = "Product 1 description",
            Price = 1
        };

        var dto = product.ToDTO();
        
        Assert.Equal(product.Id, dto.Id);
        Assert.Equal(product.Name, dto.Name);
        Assert.Equal(product.Description, dto.Description);
        Assert.Equal(product.Price, dto.Price);
    }
    
    [Fact]
    public void ToDTO_NullProduct_ShouldThrowException()
    {
        Assert.Throws<ArgumentNullException>(() => MappingExtensions.ToDTO(null));
    }
}