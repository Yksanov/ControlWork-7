namespace ControlWork7.Models;

public class BookLoan
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int BookId { get; set; }
    public DateOnly LoanDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    public DateOnly? ReturnDate { get; set; }

    public Employee Employee { get; set; }
    public Book Book { get; set; }
}