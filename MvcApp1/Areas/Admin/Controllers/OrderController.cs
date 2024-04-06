using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MvcApp1.DataAccess.Repository.IRepository;
using MvcApp1.Models;
using MvcApp1.Models.ViewModels;
using MvcApp1.Utility;
using Stripe;
using Stripe.Checkout;
using System.Security.Claims;

namespace MvcApp1.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public class OrderController : Controller
{
    private readonly IUnitOfWorks _unitOfWork;

    [BindProperty]
    public OrderVM OrderVM { get; set; }

    public OrderController(IUnitOfWorks unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Details(int orderId)
    {
        OrderVM = new()
        {
            OrderHeader = _unitOfWork.OrderHeaderRepository.Get(oh => oh.Id == orderId, includeProperties: "ApplicationUser"),
            OrderDetailList = _unitOfWork.OrderDetailRepository.GetAll(oh => oh.OrderHeaderId == orderId, includeProperties: "Product")
        };

        return View(OrderVM);
    }




    [HttpPost]
    //[Authorize(SD.Role_Admin + "," + SD.Role_Employee)]
    public IActionResult UpdateOrderDetail()
    {
        OrderHeader orderHeaderDb = _unitOfWork.OrderHeaderRepository.Get(oh => oh.Id == OrderVM.OrderHeader.Id);

        orderHeaderDb.Name = OrderVM.OrderHeader.Name;
        orderHeaderDb.Phone = OrderVM.OrderHeader.Phone;
        orderHeaderDb.StreetAddress = OrderVM.OrderHeader.StreetAddress;
        orderHeaderDb.City = OrderVM.OrderHeader.City;
        orderHeaderDb.State = OrderVM.OrderHeader.State;
        orderHeaderDb.PostalCode = OrderVM.OrderHeader.PostalCode;

        if (!string.IsNullOrEmpty(OrderVM.OrderHeader.Carrier))
        {
            orderHeaderDb.Carrier = OrderVM.OrderHeader.Carrier;
        }

        if (!string.IsNullOrEmpty(OrderVM.OrderHeader.TrackingNumber))
        {
            orderHeaderDb.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
        }

        _unitOfWork.OrderHeaderRepository.Update(orderHeaderDb);
        _unitOfWork.Save();

        return RedirectToAction(nameof(Details), new { orderId = OrderVM.OrderHeader.Id });
    }

    [HttpPost]
    public IActionResult ProcessOrder()
    {
        _unitOfWork.OrderHeaderRepository.UpdateStatus(OrderVM.OrderHeader.Id, SD.StatusInProcess);
        _unitOfWork.Save();
        TempData["success"] = "Order is in Process now";

        return RedirectToAction(nameof(Details), new { orderId = OrderVM.OrderHeader.Id });
    }

    [HttpPost]
    public IActionResult ShipOrder()
    {
        OrderHeader orderHeaderDb = _unitOfWork.OrderHeaderRepository.Get(oh => oh.Id == OrderVM.OrderHeader.Id);

        orderHeaderDb.Carrier = OrderVM.OrderHeader.Carrier;
        orderHeaderDb.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
        orderHeaderDb.OrderStatus = SD.StatusShipped;
        orderHeaderDb.ShippingDate = DateTime.Now;

        // means for company we will give 30 days to pay
        if (orderHeaderDb.PaymentStatus == SD.PaymentStatusDelayedPayment)
        {
            orderHeaderDb.PaymentDueDate = DateOnly.FromDateTime(DateTime.Now.AddDays(30));
        }

        _unitOfWork.OrderHeaderRepository.Update(orderHeaderDb);
        _unitOfWork.Save();

        TempData["success"] = "Order has been shipped";

        return RedirectToAction(nameof(Details), new { orderId = OrderVM.OrderHeader.Id });
    }

    [HttpPost]
    public IActionResult CancelOrder()
    {
        OrderHeader orderHeaderDb = _unitOfWork.OrderHeaderRepository.Get(oh => oh.Id == OrderVM.OrderHeader.Id);

        // if payment done
        if (orderHeaderDb.PaymentStatus == SD.StatusApproved)
        {
            // Need to refund as customer already paid
            var refunds = new RefundService();
            var refundOptions = new RefundCreateOptions()
            {
                Reason = RefundReasons.RequestedByCustomer,
                PaymentIntent = orderHeaderDb.PaymentIntentId
            };
            Refund refund = refunds.Create(refundOptions);

            // now updated the status
            orderHeaderDb.OrderStatus = SD.StatusRefunded;
            orderHeaderDb.PaymentStatus = SD.StatusRefunded;
        }
        else
        {
            // if payment no done yet
            orderHeaderDb.OrderStatus = SD.StatusCancelled;
            orderHeaderDb.PaymentStatus = SD.StatusCancelled;
        }

        _unitOfWork.OrderHeaderRepository.Update(orderHeaderDb);
        _unitOfWork.Save();

        TempData["success"] = "Order has been Cancelled";

        return RedirectToAction(nameof(Details), new { orderId = OrderVM.OrderHeader.Id });
    }

    [HttpPost]
    public IActionResult InitiateCompanyPayment()
    {
        OrderVM.OrderHeader = _unitOfWork.OrderHeaderRepository.Get(oh => oh.Id == OrderVM.OrderHeader.Id, includeProperties: "ApplicationUser");
        OrderVM.OrderDetailList = _unitOfWork.OrderDetailRepository.GetAll(od => od.OrderHeaderId == OrderVM.OrderHeader.Id, includeProperties: "Product");


        var domain = "https://localhost:7109/";

        var options = new Stripe.Checkout.SessionCreateOptions
        {
            SuccessUrl = domain + $"admin/order/PaymentComplete?orderId={OrderVM.OrderHeader.Id}",
            CancelUrl = domain + $"admin/order/index",
            LineItems = new List<Stripe.Checkout.SessionLineItemOptions>(),
            Mode = "payment",
        };

        foreach (var cartItem in OrderVM.OrderDetailList)
        {
            var sessionItem = new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions()
                {
                    UnitAmount = (long)(cartItem.Price * 100),
                    Currency = "usd",
                    ProductData = new SessionLineItemPriceDataProductDataOptions()
                    {
                        Name = cartItem.Product.Title
                    }
                },
                Quantity = cartItem.Count
            };

            options.LineItems.Add(sessionItem);
        }

        var service = new Stripe.Checkout.SessionService();
        Session session = service.Create(options);

        // updating SessionId and paymentIntendId
        _unitOfWork.OrderHeaderRepository.UpdateStripePaymentId(OrderVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
        _unitOfWork.Save();

        // Redirect to success url (admin/order/PaymentComplete?orderId)
        Response.Headers.Add("Location", session.Url);
        return new StatusCodeResult(303);
    }

    public IActionResult PaymentComplete(int orderId)
    {
        OrderHeader orderHeaderDb = _unitOfWork.OrderHeaderRepository.Get(oh => oh.Id == orderId, includeProperties: "ApplicationUser");

        // Stripe payment IntendId and other statuses update
        if (orderHeaderDb != null && orderHeaderDb.PaymentStatus == SD.PaymentStatusDelayedPayment)
        {
            // Company 
            var service = new SessionService();
            Session session = service.Get(orderHeaderDb.SessionId);

            if (session.PaymentStatus.ToLower() == "paid")
            {
                // updating SessionId and paymentIntendId
                _unitOfWork.OrderHeaderRepository.UpdateStripePaymentId(orderHeaderDb.Id, session.Id, session.PaymentIntentId);

                // updating order status and payment status approved
                _unitOfWork.OrderHeaderRepository.UpdateStatus(orderHeaderDb.Id, orderHeaderDb.OrderStatus, SD.PaymentStatusApproved);
                _unitOfWork.Save();
            }
        }
        TempData["success"] = "Payment completed";

        return View(orderId);
    }


    #region API CALLS

    [HttpGet]
    public IActionResult GetAll()
    {
        List<OrderHeader> orderHeaderList;

        if (User.IsInRole(SD.Role_Admin) || User.IsInRole(SD.Role_Employee))
        {
            orderHeaderList = _unitOfWork.OrderHeaderRepository.GetAll(includeProperties: "ApplicationUser").ToList();
        }
        else
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            orderHeaderList = _unitOfWork.OrderHeaderRepository.GetAll(oh => oh.ApplicationUserId == userId, includeProperties: "ApplicationUser").ToList();
        }
        return Json(new { data = orderHeaderList });
    }

    #endregion 
}

