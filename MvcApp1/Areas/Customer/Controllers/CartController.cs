using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MvcApp1.DataAccess.Repository.IRepository;
using MvcApp1.Models;
using MvcApp1.Models.ViewModels;
using MvcApp1.Utility;
using Stripe;
using Stripe.Checkout;
using System.Security.Claims;

namespace MvcApp1.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWorks _unitOfWork;

        [BindProperty]
        public ShoppingCartVM ShoppingCartVM { get; set; }

        public CartController(IUnitOfWorks unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            ShoppingCartVM = new ShoppingCartVM();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            ShoppingCartVM.ShoppingCartItemList = _unitOfWork.ShoppingCartRepository.GetAll(c => c.ApplicationUserId == userId, includeProperties: "Product");
            ShoppingCartVM.OrderHeader = new OrderHeader();

            foreach (var cartItem in ShoppingCartVM.ShoppingCartItemList)
            {
                cartItem.Price = GetPriceBasedOnQuantity(cartItem); // assigning the price here so that in cshtml we can use directly
                ShoppingCartVM.OrderHeader.OrderTotal += cartItem.Price * cartItem.Count;
            }

            return View(ShoppingCartVM);
        }

        public IActionResult Increment(int cartId)
        {
            ShoppingCart cart = _unitOfWork.ShoppingCartRepository.Get(c => c.Id == cartId);
            cart.Count += 1;
            _unitOfWork.ShoppingCartRepository.Update(cart);
            _unitOfWork.Save();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Decrement(int cartId)
        {
            ShoppingCart cart = _unitOfWork.ShoppingCartRepository.Get(c => c.Id == cartId);
            if (cart.Count <= 1)
            {
                _unitOfWork.ShoppingCartRepository.Remove(cart);
                _unitOfWork.Save();
                TempData["success"] = "The item has been removed from cart";
            }
            else
            {
                cart.Count -= 1;
                _unitOfWork.ShoppingCartRepository.Update(cart);
                _unitOfWork.Save();
            }

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Remove(int cartId)
        {
            ShoppingCart cart = _unitOfWork.ShoppingCartRepository.Get(c => c.Id == cartId);

            _unitOfWork.ShoppingCartRepository.Remove(cart);
            _unitOfWork.Save();
            TempData["success"] = "The item has been removed from cart";

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Summary()
        {
            ShoppingCartVM = new ShoppingCartVM();

            // get the Application User details
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ApplicationUser user = _unitOfWork.ApplicationUserRepository.Get(u => u.Id == userId);

            ShoppingCartVM.ShoppingCartItemList = _unitOfWork.ShoppingCartRepository.GetAll(c => c.ApplicationUserId == userId, includeProperties: "Product");
            ShoppingCartVM.OrderHeader = new OrderHeader();

            ShoppingCartVM.OrderHeader.Name = user.Name;
            ShoppingCartVM.OrderHeader.Phone = user.PhoneNumber;
            ShoppingCartVM.OrderHeader.StreetAddress = user.StreetAddress;
            ShoppingCartVM.OrderHeader.City = user.City;
            ShoppingCartVM.OrderHeader.State = user.State;
            ShoppingCartVM.OrderHeader.PostalCode = user.PostalCode;

            foreach (var cartItem in ShoppingCartVM.ShoppingCartItemList)
            {
                cartItem.Price = GetPriceBasedOnQuantity(cartItem); // assigning the price here so that in cshtml we can use directly
                ShoppingCartVM.OrderHeader.OrderTotal += cartItem.Price * cartItem.Count;
            }
            return View(ShoppingCartVM);
        }

        [HttpPost]
        [ActionName("Summary")]
        public IActionResult SummaryPOST()
        {
            // get the Application User details
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ApplicationUser user = _unitOfWork.ApplicationUserRepository.Get(u => u.Id == userId);

            // no need to instantiate ShoppingCartVM again, because we getting the it using BindProperty, from the form in the view
            ShoppingCartVM.ShoppingCartItemList = _unitOfWork.ShoppingCartRepository.GetAll(c => c.ApplicationUserId == userId, includeProperties: "Product");

            foreach (var cartItem in ShoppingCartVM.ShoppingCartItemList)
            {
                cartItem.Price = GetPriceBasedOnQuantity(cartItem); // assigning the price here so that in cshtml we can use directly
                ShoppingCartVM.OrderHeader.OrderTotal += cartItem.Price * cartItem.Count;
            }

            ShoppingCartVM.OrderHeader.ApplicationUserId = userId;
            ShoppingCartVM.OrderHeader.OrderDate = System.DateTime.Now;


            if (user.CompanyId.GetValueOrDefault() != 0)
            {
                // company user
                DateTime currentDateTime = ShoppingCartVM.OrderHeader.OrderDate;
                ShoppingCartVM.OrderHeader.PaymentDueDate = DateOnly.FromDateTime(currentDateTime).AddDays(30);

                ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusApproved;
                ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusDelayedPayment;
            }
            else
            {
                // Customer user
                ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
                ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusPending;
            }

            // adding OrderHeader
            _unitOfWork.OrderHeaderRepository.Add(ShoppingCartVM.OrderHeader);
            _unitOfWork.Save();

            // adding OrderDetails
            foreach (var cartItem in ShoppingCartVM.ShoppingCartItemList)
            {
                // configuring OrderDetails
                OrderDetail orderDetail = new()
                {
                    OrderHeaderId = ShoppingCartVM.OrderHeader.Id,
                    ProductId = cartItem.ProductId,
                    Count = cartItem.Count,
                    Price = cartItem.Price
                };

                _unitOfWork.OrderDetailRepository.Add(orderDetail);
                _unitOfWork.Save();
            }

            if (user.CompanyId.GetValueOrDefault() == 0)
            {
                // Customer payment using stripe
                var domain = "https://localhost:7109/";

                var options = new Stripe.Checkout.SessionCreateOptions
                {
                    SuccessUrl = domain + $"customer/cart/OrderComplete?id={ShoppingCartVM.OrderHeader.Id}",
                    CancelUrl = domain + $"customer/cart/index",
                    LineItems = new List<Stripe.Checkout.SessionLineItemOptions>(),
                    Mode = "payment",
                };

                foreach (var cartItem in ShoppingCartVM.ShoppingCartItemList)
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
                _unitOfWork.OrderHeaderRepository.UpdateStripePaymentId(ShoppingCartVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
                _unitOfWork.Save();

                // Redirect
                Response.Headers.Add("Location", session.Url);
                return new StatusCodeResult(303);
                // The HTTP status code 303 is a redirect status code that indicates that the requested resource can be found at a different URL. When a server responds with a 303 status code, it is telling the client (usually a web browser) to issue a GET request to the new URL provided in the response.
            }

            return RedirectToAction(nameof(OrderComplete), new { ShoppingCartVM.OrderHeader.Id });
        }


        public IActionResult OrderComplete(int id)
        {
            OrderHeader orderHeaderDb = _unitOfWork.OrderHeaderRepository.Get(oh => oh.Id == id);

            // Stripe payment IntendId and other statuses update
            if (orderHeaderDb != null && orderHeaderDb.PaymentStatus != SD.PaymentStatusDelayedPayment)
            {
                // Customer 
                var service = new SessionService();
                Session session = service.Get(orderHeaderDb.SessionId);

                if (session.PaymentStatus.ToLower() == "paid")
                {
                    // updating SessionId and paymentIntendId
                    _unitOfWork.OrderHeaderRepository.UpdateStripePaymentId(id, session.Id, session.PaymentIntentId);

                    // updating order status and payment status approved
                    _unitOfWork.OrderHeaderRepository.UpdateStatus(id, SD.StatusApproved, SD.PaymentStatusApproved);
                    _unitOfWork.Save();
                }
            }

            // delete the items from cart
            List<ShoppingCart> cartList = _unitOfWork.ShoppingCartRepository.GetAll(cart => cart.ApplicationUserId == orderHeaderDb.ApplicationUserId).ToList();

            _unitOfWork.ShoppingCartRepository.RemoveRange(cartList);
            _unitOfWork.Save();

            return View(id);
        }

        private double GetPriceBasedOnQuantity(ShoppingCart cart)
        {
            MvcApp1.Models.Product product = cart.Product;
            int count = cart.Count;

            return count switch
            {
                int n when (n > 0 && n < 50) => product.Price,
                int n when (n >= 50 && n < 100) => product.Price50,
                int n when (n >= 100) => product.Price100,
                _ => 0,
            };
        }
    }
}
