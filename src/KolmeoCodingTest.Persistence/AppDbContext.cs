using KolmeoCodingTest.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace KolmeoCodingTest.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }

    public DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>().ToTable("Products");
        modelBuilder.Entity<Product>().HasKey(x => x.Id);
        
        base.OnModelCreating(modelBuilder);
    }
}