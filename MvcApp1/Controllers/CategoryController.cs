using Microsoft.AspNetCore.Mvc;
using MvcApp1.DataAccess.Data;
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

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(Category category)
    {

        // Custom validation
        if (category.Name != null && category.DisplayOrder != 0 && category.Name.ToLower() == category.DisplayOrder.ToString())
        {
            ModelState.AddModelError("Name", "Name and Display Order can not be same");
        }

        if (category.Name != null && category.Name.ToLower() == "test")
        {
            ModelState.AddModelError("", "Please dont use test");
        }


        if (ModelState.IsValid)
        {
            _db.Categories.Add(category);
            _db.SaveChanges();
            TempData["success"] = "The category has been created successfully!";

            return RedirectToAction("Index", "Category");
        }

        return View();

    }


    public IActionResult Edit(int id)
    {
        if (id is 0)
        {
            return NotFound();
        }

        Category? catToEdit = _db.Categories.Where(category => category.Id == id).FirstOrDefault();
        if (catToEdit == null)
        {
            return NotFound();
        }

        return View(catToEdit);
    }

    [HttpPost]
    public IActionResult Edit(Category category)
    {
        if (category == null)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            _db.Categories.Update(category);
            _db.SaveChanges();
            TempData["success"] = "The category has been updated successfully!";

            return RedirectToAction("Index");

        }

        return View();
    }


    [HttpPost]
    public IActionResult Delete(int id)
    {
        if (id is 0)
        {
            return NotFound();
        }

        Category? catToDelete = _db.Categories.Where(category => category.Id == id).FirstOrDefault();
        if (catToDelete == null)
        {
            return NotFound();
        }

        _db.Categories.Remove(catToDelete);
        _db.SaveChanges();

        TempData["success"] = "The category has been deleted successfully!";

        return RedirectToAction("Index");
    }
}
