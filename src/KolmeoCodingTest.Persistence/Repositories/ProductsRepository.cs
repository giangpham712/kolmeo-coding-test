using KolmeoCodingTest.Core.Domain.Entities;
using KolmeoCodingTest.Core.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace KolmeoCodingTest.Persistence.Repositories;

public class ProductsRepository : IProductsRepository
{
    private readonly AppDbContext _context;

    public ProductsRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Product?> Get(int id)
    {
        return await _context.Products.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task Update(Product product)
    {
        _context.Products.Update(product);
        await _context.SaveChangesAsync();
    }

    public async Task Create(Product product)
    {
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
    }

    public async Task Delete(Product product)
    {
        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Product>> List()
    {
        return await _context.Products.AsNoTracking().ToListAsync();
    }
}