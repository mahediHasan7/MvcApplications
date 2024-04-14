using MahediBookStore.Models;

namespace MahediBookStore.DataAccess.Repository.IRepository;

public interface IShoppingCartRepository : IRepository<ShoppingCart>
{
    public void Update(ShoppingCart Cart);
}
