using ControlWork7.Models;

namespace ControlWork7.ViewModels;

public class BookModels
{
    public IEnumerable<Book> Book { get; set; } //Books дебе
    public IEnumerable<Category> Categories { get; set; }
    public PageViewModel PageViewModel { get; set; }
}