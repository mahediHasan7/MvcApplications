using MvcApp1.Models;

namespace MvcApp1.DataAccess.Repository.IRepository;

public interface IShoppingCartRepository : IRepository<ShoppingCart>
{
    public void Update(ShoppingCart Cart);
}
