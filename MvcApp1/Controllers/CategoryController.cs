using Microsoft.AspNetCore.Mvc;
using MvcApp1.Data;
using MvcApp1.Models;

namespace MvcApp1.Controllers;

public class CategoryController : Controller
{
    private readonly ApplicationDbContext _db;
    public CategoryController(ApplicationDbContext db)
    {
        _db = db;
    }

    public IActionResult Index()
    {
        // Retrieve the data from database
        List<Category> categories = _db.Categories.ToList();

        return View(categories);
    }

}
