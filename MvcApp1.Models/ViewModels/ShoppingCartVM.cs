using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace MahediBookStore.Models.ViewModels;

public class ShoppingCartVM
{
    [ValidateNever]
    public IEnumerable<ShoppingCart> ShoppingCartItemList { get; set; }
    public OrderHeader OrderHeader { get; set; }
}
