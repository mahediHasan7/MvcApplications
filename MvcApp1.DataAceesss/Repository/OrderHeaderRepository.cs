using MvcApp1.DataAccess.Data;
using MvcApp1.DataAccess.Repository.IRepository;
using MvcApp1.Models;

namespace MvcApp1.DataAccess.Repository;

public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
{
    private readonly ApplicationDbContext _context;

    public OrderHeaderRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }
    public void Update(OrderHeader orderHeader)
    {
        _context.OrderHeaders.Update(orderHeader);
    }

    public void UpdateStatus(int id, string orderStatus, string? paymentStatus = null)
    {
        var orderHeaderDb = _context.OrderHeaders.FirstOrDefault(oh => oh.Id == id);
        if (orderHeaderDb != null)
        {
            orderHeaderDb.OrderStatus = orderStatus;
            if (paymentStatus != null)
            {
                orderHeaderDb.PaymentStatus = paymentStatus;
            }
        }
    }

    public void UpdateStripePaymentId(int id, string sessionId, string paymentIntendId)
    {
        var orderHeaderDb = _context.OrderHeaders.FirstOrDefault(oh => oh.Id == id);
        if (orderHeaderDb != null)
        {
            if (!string.IsNullOrEmpty(sessionId))
            {
                orderHeaderDb.SessionId = sessionId;
            }
            if (!string.IsNullOrEmpty(paymentIntendId))
            {

                orderHeaderDb.PaymentIntentId = paymentIntendId;
                orderHeaderDb.PaymentDate = DateTime.Now;
            }
        }
    }
}
