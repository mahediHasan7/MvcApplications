using Microsoft.AspNetCore.Mvc;
using MvcApp1.DataAccess.Data;
using MvcApp1.DataAccess.Repository.IRepository;
using MvcApp1.Models;
using System.Diagnostics;

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
            IEnumerable<Product> products = _unitOfWork.ProductRepository.GetAll("Category");
            return View(products);
        }

        public IActionResult Details(int productId)
        {
            Product product = _unitOfWork.ProductRepository.Get(p => p.Id == productId, "Category");
            return View(product);
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
