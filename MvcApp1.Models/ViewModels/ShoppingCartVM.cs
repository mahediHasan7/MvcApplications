using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace MvcApp1.Models.ViewModels;

public class ShoppingCartVM
{
    [ValidateNever]
    public IEnumerable<ShoppingCart> ShoppingCartItemList { get; set; }
    public double CartTotal { get; set; }
}
