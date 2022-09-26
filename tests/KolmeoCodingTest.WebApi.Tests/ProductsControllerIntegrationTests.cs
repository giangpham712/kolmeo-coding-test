using System.Net;
using System.Text;
using System.Text.Json;
using KolmeoCodingTest.Application.Contracts.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;

namespace KolmeoCodingTest.WebApi.Tests;

public class ProductsControllerIntegrationTests
{
    [Fact]
    public async Task List_NoProducts_ShouldReturnEmptyListJson()
    {
        var httpClient = new WebApplicationFactory<Program>().Server.CreateClient();
        
        var response = await httpClient.GetAsync("/products");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var json = await response.Content.ReadAsStringAsync();
        var products = JsonSerializer.Deserialize<IEnumerable<ProductDTO>>(json, DefaultJsonSerializerOptions);
        
        Assert.NotNull(products);
        
    }
    
    [Fact]
    public async Task Create_ValidData_ShouldCreateProductSuccessfully()
    {
        var httpClient = new WebApplicationFactory<Program>().Server.CreateClient();

        var productData = new
        {
            Name = "Test product 1",
            Description = "Description for Test product 1",
            Price = 100
        };
        
        var productJson = JsonSerializer.Serialize(productData, DefaultJsonSerializerOptions);
        
        var response = await httpClient.PostAsync("/products", new StringContent(productJson, Encoding.UTF8, "application/json"));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        
        var json = await response.Content.ReadAsStringAsync();
        var createdProduct = JsonSerializer.Deserialize<ProductDTO>(json, DefaultJsonSerializerOptions);
        
        Assert.NotNull(createdProduct);
        
        Assert.Equal(productData.Name, createdProduct.Name);
        Assert.Equal(productData.Description, createdProduct.Description);
        Assert.Equal(productData.Price, createdProduct.Price);
    }
    
    [Fact]
    public async Task Create_EmptyName_ShouldReturnValidationError()
    {
        var httpClient = new WebApplicationFactory<Program>().Server.CreateClient();

        var productData = new
        {
            Name = "",
            Description = "Description for Test product 1",
            Price = 100
        };
        
        var productJson = JsonSerializer.Serialize(productData, DefaultJsonSerializerOptions);
        
        var response = await httpClient.PostAsync("/products", new StringContent(productJson, Encoding.UTF8, "application/json"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        
        var json = await response.Content.ReadAsStringAsync();
        var errorDetails = JsonSerializer.Deserialize<ProblemDetails>(json, DefaultJsonSerializerOptions);
        
        Assert.NotNull(errorDetails);
        
        Assert.Equal("Validation error occurred.", errorDetails.Title);
        Assert.Equal("Product name must not be empty.", errorDetails.Detail);
    }
    
    [Fact]
    public async Task Create_NegativePrice_ShouldReturnValidationError()
    {
        var httpClient = new WebApplicationFactory<Program>().Server.CreateClient();

        var productData = new
        {
            Name = "Test product 1",
            Description = "Description for Test product 1",
            Price = -100
        };
        
        var productJson = JsonSerializer.Serialize(productData, DefaultJsonSerializerOptions);
        
        var response = await httpClient.PostAsync("/products", new StringContent(productJson, Encoding.UTF8, "application/json"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        
        var json = await response.Content.ReadAsStringAsync();
        var errorDetails = JsonSerializer.Deserialize<ProblemDetails>(json, DefaultJsonSerializerOptions);
        
        Assert.NotNull(errorDetails);
        
        Assert.Equal("Validation error occurred.", errorDetails.Title);
        Assert.Equal("Product price must not be negative.", errorDetails.Detail);
    }

    private JsonSerializerOptions DefaultJsonSerializerOptions => new JsonSerializerOptions()
        { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
}