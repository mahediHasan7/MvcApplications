using MvcApp1.Models;

namespace MvcApp1.DataAccess.Repository.IRepository;

public interface IOrderHeaderRepository : IRepository<OrderHeader>
{
    public void Update(OrderHeader orderHeader);
    public void UpdateStatus(int id, string orderStatus, string? paymentStatus = null);
    public void UpdateStripePaymentId(int id, string sessionId, string paymentIntendId);
}
