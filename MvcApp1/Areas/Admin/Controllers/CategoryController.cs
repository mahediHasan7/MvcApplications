using Microsoft.AspNetCore.Mvc;
using MvcApp1.DataAccess.Repository.IRepository;
using MvcApp1.Models;


namespace MvcApp1.Areas.Admin.Controllers;

[Area("Admin")]
public class CategoryController : Controller
{
    private readonly IUnitOfWorks _unitOfWork;
    public CategoryController(IUnitOfWorks unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public IActionResult Index()
    {
        // Retrieve the data from database
        List<Category> categories = _unitOfWork.CategoryRepository.GetAll().ToList();

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
            _unitOfWork.CategoryRepository.Add(category);
            _unitOfWork.Save();
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

        Category? catToEdit = _unitOfWork.CategoryRepository.Get(cat => cat.Id == id);
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
            _unitOfWork.CategoryRepository.Update(category);
            _unitOfWork.Save();

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

        Category? catToDelete = _unitOfWork.CategoryRepository.Get(cat => cat.Id == id);
        if (catToDelete == null)
        {
            return NotFound();
        }

        _unitOfWork.CategoryRepository.Remove(catToDelete);
        _unitOfWork.Save();

        TempData["success"] = "The category has been deleted successfully!";

        return RedirectToAction("Index");
    }
}
