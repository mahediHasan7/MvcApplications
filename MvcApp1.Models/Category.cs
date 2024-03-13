using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace MvcApp1.Models;

[DebuggerDisplay("{Name}")]
public class Category
{
    public int Id { get; set; }

    [Required]
    [DisplayName("Category Name")]
    [MaxLength(30)]
    public string? Name { get; set; }

    [DisplayName("Display Order")]
    [Range(1, 100, ErrorMessage = "The number need to be 1-100")]
    public int DisplayOrder { get; set; }
}
