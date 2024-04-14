using MahediBookStore.Models;

namespace MahediBookStore.DataAccess.Repository.IRepository;

public interface IOrderDetailRepository : IRepository<OrderDetail>
{
    public void Update(OrderDetail orderDetail);
}
