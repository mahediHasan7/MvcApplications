using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace MvcApp1.Models;

[DebuggerDisplay("{Name}")]
public class Category
{
    public int Id { get; set; }
    [Required]
    public required string Name { get; set; }
    public int DisplayOrder { get; set; }

}
