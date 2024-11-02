using System.ComponentModel.DataAnnotations;

namespace ControlWork7.ViewModels;

public class RegisterViewModel
{
    [Required(ErrorMessage = "First Name is required")]
    [StringLength(100,MinimumLength = 3, ErrorMessage = "The first name must be between from 3 to 100 characters")]
    public string FirstName { get; set; }
    
    [Required(ErrorMessage = "Last Name is required")]
    [StringLength(100,MinimumLength = 3, ErrorMessage = "The last name must be between from 3 to 100 characters")]
    public string LastName { get; set; }
    
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid Email Address")]
    public string Email { get; set; }
    
    [Required(ErrorMessage = "Phone Number is required")]
    [StringLength(10,MinimumLength = 0, ErrorMessage = "The phone number must be between from 0 to 10 characters")]
    public string PhoneNumber { get; set; }
    
    [Required(ErrorMessage = "Password is required")]
    [DataType(DataType.Password)]
    public string Password { get; set; }
    
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Пароли не совпадают")]
    public string ConfirmPassword { get; set; }
}