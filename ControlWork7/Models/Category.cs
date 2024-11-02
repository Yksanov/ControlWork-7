using System.ComponentModel.DataAnnotations;

namespace ControlWork7.Models;

public class Category
{
    public int Id { get; set; }
    [Required(ErrorMessage = "Category Name is required")]
    [StringLength(100,MinimumLength = 3, ErrorMessage = "The category name must be between from 3 to 100 characters")]
    public string Name { get; set; }
    
}