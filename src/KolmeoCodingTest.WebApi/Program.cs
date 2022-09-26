using KolmeoCodingTest.Application;
using KolmeoCodingTest.Application.Contracts.Interfaces;
using KolmeoCodingTest.Core.Domain.Repositories;
using KolmeoCodingTest.Core.Domain.Services;
using KolmeoCodingTest.Persistence;
using KolmeoCodingTest.Persistence.Repositories;
using KolmeoCodingTest.WebApi.Errors;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseInMemoryDatabase("InMemory");
});

builder.Services.AddScoped<IProductManager, ProductManager>();
builder.Services.AddScoped<IProductsService, ProductsService>();
builder.Services.AddScoped<IProductsRepository, ProductsRepository>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ErrorHandlerMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program {}