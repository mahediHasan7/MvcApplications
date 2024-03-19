using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MvcApp1.DataAccess.Repository.IRepository;
using MvcApp1.Models;
using MvcApp1.Models.ViewModels;

namespace MvcApp1.Areas.Admin.Controllers;

[Area("Admin")]
public class ProductController : Controller
{
    private readonly IUnitOfWorks _unitOfWork;
    private readonly IWebHostEnvironment _env;

    public ProductController(IUnitOfWorks unitOfWork, IWebHostEnvironment env)
    {
        _unitOfWork = unitOfWork;
        _env = env;
    }

    public IActionResult Index()
    {
        List<Product> products = _unitOfWork.ProductRepository.GetAll("Category").ToList();
        return View(products);
    }

    public IActionResult CreateOrUpdate(int? id)
    {
        // Make IEnumerable of SelectListItem (Class in ASP.NET CORE) for select Items in a dropdown list / <select> element in HTML
        IEnumerable<SelectListItem> CategoryList = _unitOfWork.CategoryRepository.GetAll().Select(cat => new SelectListItem()
        {
            Text = cat.Name,
            Value = cat.Id.ToString()
        });

        ProductVM productVm = new()
        {
            Product = new Product(),
            CategoryList = CategoryList
        };

        if (id == 0 || id == null)
        {
            return View(productVm);
        }
        else
        {
            // Update
            Product product = _unitOfWork.ProductRepository.Get(p => p.Id == id);
            productVm.Product = product;

            return View(productVm);
        }

    }

    [HttpPost]
    public async Task<IActionResult> CreateOrUpdate(ProductVM productVm, IFormFile? file)
    {
        if (ModelState.IsValid)
        {
            // Saving the image in the wwwroot/images/product folder
            // setting the image path to ImageUrl
            if (file != null)
            {
                // getting the path of wwwroot/images/product
                string pathToImages = Path.Combine(_env.WebRootPath, "images", "product");

                // make an image name with extension
                string imageName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

                // constructing the path to save the image
                string imagePath = Path.Combine(pathToImages, imageName);

                // Remove the old image before saving the new
                if (string.IsNullOrEmpty(productVm.Product.ImageUrl) == false)
                {
                    // extract the image name from the ImageUrl
                    string oldImageName = Path.GetFileName(productVm.Product.ImageUrl);
                    string oldImagePath = Path.Combine(_env.WebRootPath, "images", "product", oldImageName);

                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                // saving the image to the imagePath
                using (var fileStream = new FileStream(imagePath, FileMode.CreateNew))
                {
                    // saving the image
                    await file.CopyToAsync(fileStream);
                }

                // update ImageUrl
                productVm.Product.ImageUrl = Path.Combine("images", "product", imageName);
            }

            // create product
            if (productVm.Product.Id == 0)
            {
                _unitOfWork.ProductRepository.Add(productVm.Product);
                _unitOfWork.Save();

                TempData["success"] = "The product has created!";
            }
            // update product
            else
            {
                _unitOfWork.ProductRepository.Update(productVm.Product);
                _unitOfWork.Save();
                TempData["success"] = "The product has updated!";
            }



            return RedirectToAction("Index");
        }


        // If a any input data was invalid, then we need to re-populate the CategoryList
        IEnumerable<SelectListItem> CategoryList = _unitOfWork.CategoryRepository.GetAll().Select(cat => new SelectListItem()
        {
            Text = cat.Name,
            Value = cat.Id.ToString()
        });

        productVm.CategoryList = CategoryList;


        return View(productVm);
    }

    public IActionResult Edit(int id)
    {
        Product product = _unitOfWork.ProductRepository.Get(p => p.Id == id);

        return View(product);
    }

    [HttpPost]
    public IActionResult Edit(Product product)
    {
        _unitOfWork.ProductRepository.Update(product);
        _unitOfWork.Save();

        TempData["success"] = "The product has updated!";

        return RedirectToAction("Index");
    }


    #region API CALLS

    [HttpGet]
    public IActionResult GetAll()
    {
        List<Product> products = _unitOfWork.ProductRepository.GetAll("Category").ToList();
        return Json(new { data = products });
    }

    [HttpDelete]
    public IActionResult Delete(int id)
    {

        Product product = _unitOfWork.ProductRepository.Get(p => p.Id == id);

        if (product == null)
        {
            return Json(new
            {
                success = false,
                message = "Error while deleting!"
            });
        }


        // delete the image
        if (string.IsNullOrEmpty(product.ImageUrl) == false)
        {
            // extract the image name from the ImageUrl
            string oldImageName = Path.GetFileName(product.ImageUrl);
            string oldImagePath = Path.Combine(_env.WebRootPath, "images", "product", oldImageName);

            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }
        }

        // delete the product
        _unitOfWork.ProductRepository.Remove(product);
        _unitOfWork.Save();

        return Json(new
        {
            success = true,
            message = "Product has been deleted!"
        });

    }

    #endregion

}