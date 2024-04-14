using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MahediBookStore.Models;

public class OrderHeader
{
    // Navigation properties in ASP.NET are properties that allow you to navigate and establish relationships between entities in an object-oriented manner.

    public int Id { get; set; }

    public string ApplicationUserId { get; set; }
    [ForeignKey("ApplicationUserId")]
    [ValidateNever]
    public ApplicationUser ApplicationUser { get; set; }

    public DateTime OrderDate { get; set; }
    public DateTime ShippingDate { get; set; }
    public DateTime PaymentDate { get; set; }
    public DateOnly PaymentDueDate { get; set; } // for company who can buy without paying (30 days)

    public double OrderTotal { get; set; }

    public string? OrderStatus { get; set; }
    public string? PaymentStatus { get; set; }
    public string? TrackingNumber { get; set; }
    public string? Carrier { get; set; }

    public string? SessionId { get; set; } // for stripe 
    public string? PaymentIntentId { get; set; } // for stripe 

    [Required]
    public string Name { get; set; }
    [Required]
    public string Phone { get; set; }
    [Required]
    public string StreetAddress { get; set; }
    [Required]
    public string City { get; set; }
    [Required]
    public string State { get; set; }
    [Required]
    public string PostalCode { get; set; }
}
