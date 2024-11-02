using ControlWork7.Models;

namespace ControlWork7.ViewModels;

public class BookModels
{
    public IEnumerable<Book> Book { get; set; } //Books дебе
    public PageViewModel PageViewModel { get; set; }
}