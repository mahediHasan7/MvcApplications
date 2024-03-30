using Microsoft.AspNetCore.Mvc;
using MvcApp1.DataAccess.Data;
using MvcApp1.DataAccess.Repository.IRepository;
using MvcApp1.Models;
using System.Diagnostics;
using System.Security.Claims;

namespace MvcApp1.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWorks _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWorks unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> products = _unitOfWork.ProductRepository.GetAll(includeProperties: "Category");
            return View(products);
        }

        public IActionResult Details(int productId)
        {
            Product product = _unitOfWork.ProductRepository.Get(p => p.Id == productId, "Category");

            ShoppingCart cart = new()
            {
                ProductId = product.Id,
                Product = product,
                Count = 1,
                ApplicationUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
            };

            return View(cart);
        }

        [HttpPost]
        public IActionResult Details(ShoppingCart cart)
        {

            // check if the userId and productId combination exists for any shopping cart in DB
            ShoppingCart existingCartItem = _unitOfWork.ShoppingCartRepository.Get(c => c.ProductId == cart.ProductId && c.ApplicationUserId == cart.ApplicationUserId);

            if (existingCartItem is null)
            {
                _unitOfWork.ShoppingCartRepository.Add(cart);

            }
            else
            {
                existingCartItem.Count += cart.Count;
                _unitOfWork.ShoppingCartRepository.Update(existingCartItem);
            }

            _unitOfWork.Save();
            TempData["success"] = "Cart updated successfully!";

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
