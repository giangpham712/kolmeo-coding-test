using KolmeoCodingTest.Core.Domain.Entities;

namespace KolmeoCodingTest.Core.Domain.Repositories
{
    public interface IProductsRepository
    {
        Task<Product?> Get(int id);
        Task Update(Product product);
        Task Create(Product product);
        Task Delete(Product product);
        Task<IEnumerable<Product>> List();
    }
}