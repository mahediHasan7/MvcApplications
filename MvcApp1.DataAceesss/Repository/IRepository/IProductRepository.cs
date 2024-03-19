using MvcApp1.Models;

namespace MvcApp1.DataAccess.Repository.IRepository;

public interface IProductRepository : IRepository<Product>
{
    void Update(Product product);
}
