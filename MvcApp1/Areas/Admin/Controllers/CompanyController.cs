using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MvcApp1.DataAccess.Repository.IRepository;
using MvcApp1.Models;
using MvcApp1.Utility;

namespace MvcApp1.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = SD.Role_Admin)]
public class CompanyController : Controller
{
    private readonly IUnitOfWorks _unitOfWork;

    public CompanyController(IUnitOfWorks unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public IActionResult Index()
    {
        List<Models.Company> companies = _unitOfWork.CompanyRepository.GetAll().ToList();
        return View(companies);
    }

    public IActionResult CreateOrUpdate(int? id)
    {
        if (id == 0 || id == null)
        {
            return View(new Company());
        }
        else
        {
            Company company = _unitOfWork.CompanyRepository.Get(c => c.Id == id);
            return View(company);
        }

    }

    [HttpPost]
    public IActionResult CreateOrUpdate(Company company)
    {
        if (ModelState.IsValid)
        {

            // create 
            if (company.Id == 0)
            {
                _unitOfWork.CompanyRepository.Add(company);
                TempData["success"] = "The company has created!";
            }
            // update 
            else
            {
                _unitOfWork.CompanyRepository.Update(company);
                TempData["success"] = "The company has updated!";
            }

            _unitOfWork.Save();
            return RedirectToAction("Index");
        }

        return View(company);
    }


    #region API CALLS

    [HttpGet]
    public IActionResult GetAll()
    {
        List<Company> companies = _unitOfWork.CompanyRepository.GetAll().ToList();
        return Json(new { data = companies });
    }

    [HttpDelete]
    public IActionResult Delete(int id)
    {

        Company company = _unitOfWork.CompanyRepository.Get(comp => comp.Id == id);

        if (company == null)
        {
            return Json(new
            {
                success = false,
                message = "Error while deleting!"
            });
        }


        // delete  
        _unitOfWork.CompanyRepository.Remove(company);
        _unitOfWork.Save();

        return Json(new
        {
            success = true,
            message = "Company has been deleted!"
        });

    }

    #endregion

}