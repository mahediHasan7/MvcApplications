using Microsoft.AspNetCore.Mvc;
using MahediBookStore.DataAccess.Repository.IRepository;
using MahediBookStore.Utility;
using System.Security.Claims;

namespace MahediBookStore.ViewComponents
{
    public class CartItemCountViewComponent : ViewComponent
    {
        private readonly IUnitOfWorks _unitOfWork;

        public CartItemCountViewComponent(IUnitOfWorks unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var claimsPrincipal = User as ClaimsPrincipal;
            var userId = claimsPrincipal?.FindFirstValue(ClaimTypes.NameIdentifier);

            int cartItemCountForUser = _unitOfWork.ShoppingCartRepository.GetAll(c => c.ApplicationUserId == userId).Count();

            if (cartItemCountForUser != null)
            {
                HttpContext.Session.SetInt32(SD.CartSession, cartItemCountForUser);
                return View(HttpContext.Session.GetInt32(SD.CartSession));
            }
            else
            {
                //clear the session
                HttpContext.Session.Remove(SD.CartSession);
                return View(0);

            }

        }
    }
}
