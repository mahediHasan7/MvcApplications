using MvcApp1.Models;

namespace MvcApp1.DataAccess.Repository.IRepository;

public interface IOrderHeaderRepository : IRepository<OrderHeader>
{
    public void Update(OrderHeader orderHeader);
}
