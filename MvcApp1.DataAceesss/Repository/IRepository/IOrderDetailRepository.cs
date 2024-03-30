using MvcApp1.Models;

namespace MvcApp1.DataAccess.Repository.IRepository;

public interface IOrderDetailRepository : IRepository<OrderDetail>
{
    public void Update(OrderDetail orderDetail);
}
