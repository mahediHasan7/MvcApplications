using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MvcApp1.DataAccess.Repository.IRepository;
using MvcApp1.Models;
using MvcApp1.Models.ViewModels;
using System.Security.Claims;

namespace MvcApp1.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWorks _unitOfWork;
        private readonly ShoppingCartVM shoppingCartVM;

        public CartController(IUnitOfWorks unitOfWork)
        {
            _unitOfWork = unitOfWork;
            shoppingCartVM = new();
        }

        public IActionResult Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            shoppingCartVM.ShoppingCartItemList = _unitOfWork.ShoppingCartRepository.GetAll(c => c.ApplicationUserId == userId, includeProperties: "Product");

            foreach (var cartItem in shoppingCartVM.ShoppingCartItemList)
            {
                cartItem.Price = GetPriceBasedOnQuantity(cartItem); // assigning the price here so that in cshtml we can use directly
                shoppingCartVM.CartTotal += cartItem.Price * cartItem.Count;
            }

            return View(shoppingCartVM);
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
            return View();
        }

        private double GetPriceBasedOnQuantity(ShoppingCart cart)
        {
            Product product = cart.Product;
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
