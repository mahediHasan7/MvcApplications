using MvcApp1.DataAccess.Data;
using MvcApp1.DataAccess.Repository.IRepository;
using MvcApp1.Models;

namespace MvcApp1.DataAccess.Repository;

internal class ProductRepository : Repository<Product>, IProductRepository
{
    private readonly ApplicationDbContext _context;

    public ProductRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public void Update(Product product)
    {
        _context.Products.Update(product);
    }
}
