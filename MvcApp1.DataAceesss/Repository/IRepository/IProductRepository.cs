using MahediBookStore.Models;

namespace MahediBookStore.DataAccess.Repository.IRepository;

public interface IProductRepository : IRepository<Product>
{
    void Update(Product product);
}
