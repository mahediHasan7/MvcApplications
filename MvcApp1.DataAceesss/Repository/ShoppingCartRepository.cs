using MahediBookStore.DataAccess.Data;
using MahediBookStore.DataAccess.Repository.IRepository;
using MahediBookStore.Models;

namespace MahediBookStore.DataAccess.Repository;

internal class ShoppingCartRepository : Repository<ShoppingCart>, IShoppingCartRepository
{
    private readonly ApplicationDbContext _context;

    public ShoppingCartRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public void Update(ShoppingCart cart)
    {
        _context.ShoppingCarts.Update(cart);
    }
}
