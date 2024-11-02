using System.ComponentModel.DataAnnotations;
using System.Drawing.Printing;
using ControlWork7.Services;

namespace ControlWork7.Models;

public class Book
{
    public int Id { get; set; }
    [Required(ErrorMessage = "Title is required")]
    public string Name { get; set; }
    [Required(ErrorMessage = "Author is required")]
    public string Author { get; set; }
    [Required(ErrorMessage = "Cover Image url is required")]
    public string CoverImageUrl { get; set; }
    public int YearPublished { get; set; }
    public string Description { get; set; }
    public DateOnly CreatedDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    public Status Status { get; set; }
    
    public int? CategoryId { get; set; }
    public Category? Category { get; set; }
}