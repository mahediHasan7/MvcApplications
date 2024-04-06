using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace MvcApp1.Models.ViewModels;

public class OrderVM
{
    public OrderHeader OrderHeader { get; set; }
    public IEnumerable<OrderDetail> OrderDetailList { get; set; }
}
