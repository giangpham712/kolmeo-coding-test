using System.Net;
using System.Text;
using System.Text.Json;
using KolmeoCodingTest.Application.Contracts.DTOs;
using KolmeoCodingTest.Core.Domain.Entities;
using KolmeoCodingTest.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace KolmeoCodingTest.WebApi.Tests;

public class ProductsControllerIntegrationTests: IAsyncLifetime
{
    public async Task InitializeAsync()
    {
        var applicationFactory = new WebApplicationFactory<Program>();
        var scope = applicationFactory.Services.GetService<IServiceScopeFactory>()!.CreateScope();
        var appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        
        foreach (var product in appDbContext.Products)
        {
            appDbContext.Products.Remove(product);
        }

        await appDbContext.SaveChangesAsync();
    }
    
    #region List
    
    [Fact]
    public async Task List_NoProducts_ShouldReturnEmptyList()
    {
        var httpClient = new WebApplicationFactory<Program>().Server.CreateClient();
        
        var response = await httpClient.GetAsync("/products");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var json = await response.Content.ReadAsStringAsync();
        var products = JsonSerializer.Deserialize<IEnumerable<ProductDTO>>(json, DefaultJsonSerializerOptions);
        
        Assert.NotNull(products);
        Assert.Empty(products);
    }
    
    [Fact]
    public async Task List_HasProducts_ShouldReturnProductList()
    {
        var applicationFactory = new WebApplicationFactory<Program>();
        var scope = applicationFactory.Services.GetService<IServiceScopeFactory>()!.CreateScope();
        var appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var currentProducts = new [] {
            new Product()
            {
                Name = "Test product 1",
                Description = "Description for Test product 1",
                Price = 100
            },
            new Product()
            {
                Name = "Test product 2",
                Description = "Description for Test product 2",
                Price = 200
            },
            new Product()
            {
                Name = "Test product 3",
                Description = "Description for Test product 3",
                Price = 300
            }
        };

        appDbContext.Products.AddRange(currentProducts);
        await appDbContext.SaveChangesAsync();
        
        var httpClient = new WebApplicationFactory<Program>().Server.CreateClient();
        
        var response = await httpClient.GetAsync("/products");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var json = await response.Content.ReadAsStringAsync();
        var products = JsonSerializer.Deserialize<IEnumerable<ProductDTO>>(json, DefaultJsonSerializerOptions);
        
        Assert.NotNull(products);
        Assert.Equal(3, products.Count());
    }
    
    #endregion
    
    #region Create
    
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
        var problem = JsonSerializer.Deserialize<ProblemDetails>(json, DefaultJsonSerializerOptions);
        
        Assert.NotNull(problem);
        
        Assert.Equal("Validation error occurred.", problem.Title);
        Assert.Equal("Product name must not be empty.", problem.Detail);
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

    #endregion
    
    #region Get
    
    [Fact]
    public async Task Get_ValidProduct_ShouldReturnProduct()
    {
        var applicationFactory = new WebApplicationFactory<Program>();
        var scope = applicationFactory.Services.GetService<IServiceScopeFactory>()!.CreateScope();
        var appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var product = new Product()
        {
            Name = "Test product 1",
            Description = "Description for Test product 1",
            Price = 100
        };
        
        appDbContext.Products.Add(product);
        await appDbContext.SaveChangesAsync();
        
        var httpClient = new WebApplicationFactory<Program>().Server.CreateClient();
        
        var response = await httpClient.GetAsync($"/products/{product.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var json = await response.Content.ReadAsStringAsync();
        var existingProduct = JsonSerializer.Deserialize<ProductDTO>(json, DefaultJsonSerializerOptions);
        
        Assert.NotNull(existingProduct);
        Assert.Equal(product.Id, existingProduct.Id);
        Assert.Equal(product.Name, existingProduct.Name);
        Assert.Equal(product.Description, existingProduct.Description);
        Assert.Equal(product.Price, existingProduct.Price);
    }
    
    [Fact]
    public async Task Get_InvalidProduct_ShouldReturnNotFoundError()
    {
        var httpClient = new WebApplicationFactory<Program>().Server.CreateClient();
        
        var response = await httpClient.GetAsync("/products/123");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        
        var json = await response.Content.ReadAsStringAsync();
        var problem = JsonSerializer.Deserialize<ProblemDetails>(json, DefaultJsonSerializerOptions);
        
        Assert.NotNull(problem);
        Assert.Equal("Product with ID 123 could not be found.", problem.Title);
    }
    
    #endregion
    
    #region Update
    
    [Fact]
    public async Task Update_ValidProductAndValidData_ShouldUpdateProduct()
    {
        var applicationFactory = new WebApplicationFactory<Program>();
        var scope = applicationFactory.Services.GetService<IServiceScopeFactory>()!.CreateScope();
        var appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var product = new Product()
        {
            Name = "Test product 1",
            Description = "Description for Test product 1",
            Price = 100
        };
        
        appDbContext.Products.Add(product);
        await appDbContext.SaveChangesAsync();
        
        var productUpdateData = new
        {
            Name = "Updated test product 1",
            Description = "Updated description for Test product 1",
            Price = 200
        };
        
        var productJson = JsonSerializer.Serialize(productUpdateData, DefaultJsonSerializerOptions);
        
        var httpClient = new WebApplicationFactory<Program>().Server.CreateClient();
        
        var response = await httpClient.PutAsync($"/products/{product.Id}", new StringContent(productJson, Encoding.UTF8, "application/json"));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var json = await response.Content.ReadAsStringAsync();
        var updatedProduct = JsonSerializer.Deserialize<ProductDTO>(json, DefaultJsonSerializerOptions);
        
        Assert.NotNull(updatedProduct);
        Assert.Equal(product.Id, updatedProduct.Id);
        Assert.Equal(productUpdateData.Name, updatedProduct.Name);
        Assert.Equal(productUpdateData.Description, updatedProduct.Description);
        Assert.Equal(productUpdateData.Price, updatedProduct.Price);
    }
    
    [Fact]
    public async Task Update_InvalidProduct_ShouldReturnNotFoundError()
    {
        var productUpdateData = new
        {
            Name = "Updated test product 1",
            Description = "Updated description for Test product 1",
            Price = 200
        };
        
        var productJson = JsonSerializer.Serialize(productUpdateData, DefaultJsonSerializerOptions);
        
        var httpClient = new WebApplicationFactory<Program>().Server.CreateClient();
        
        var response = await httpClient.PutAsync("/products/123", new StringContent(productJson, Encoding.UTF8, "application/json"));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        
        var json = await response.Content.ReadAsStringAsync();
        var problem = JsonSerializer.Deserialize<ProblemDetails>(json, DefaultJsonSerializerOptions);
        
        Assert.NotNull(problem);
        Assert.Equal("Product with ID 123 could not be found.", problem.Title);
    }
    
    #endregion
    
    #region Delete
    
    [Fact]
    public async Task Delete_ValidProduct_ShouldDeleteProduct()
    {
        var applicationFactory = new WebApplicationFactory<Program>();
        var scope = applicationFactory.Services.GetService<IServiceScopeFactory>()!.CreateScope();
        var appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var product = new Product()
        {
            Name = "Test product 1",
            Description = "Description for Test product 1",
            Price = 100
        };
        
        appDbContext.Products.Add(product);
        await appDbContext.SaveChangesAsync();
        
        var httpClient = new WebApplicationFactory<Program>().Server.CreateClient();
        
        var response = await httpClient.DeleteAsync($"/products/{product.Id}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var deletedProduct = await appDbContext.Products.FirstOrDefaultAsync(x => x.Id == product.Id);
        Assert.Null(deletedProduct);
    }
    
    [Fact]
    public async Task Delete_InvalidProduct_ShouldReturnNotFoundError()
    {
        var httpClient = new WebApplicationFactory<Program>().Server.CreateClient();
        
        var response = await httpClient.DeleteAsync("/products/123");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        
        var json = await response.Content.ReadAsStringAsync();
        var problem = JsonSerializer.Deserialize<ProblemDetails>(json, DefaultJsonSerializerOptions);
        
        Assert.NotNull(problem);
        Assert.Equal("Product with ID 123 could not be found.", problem.Title);
    }

    #endregion
    
    private JsonSerializerOptions DefaultJsonSerializerOptions => new JsonSerializerOptions()
        { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    

    public async Task DisposeAsync()
    {
        //
    }
}