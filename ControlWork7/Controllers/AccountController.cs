using System.Security.Claims;
using ControlWork7.Models;
using ControlWork7.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ControlWork7.Controllers;

public class AccountController : Controller
{
    private readonly LibraryContext _context;

    public AccountController(LibraryContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            Employee employee = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email && u.Password == model.Password);
            if (employee != null)
            {
                await AuthenticateAsync(model.Email); // аутентификация
                return RedirectToAction("Index", "Book");
            }
            ModelState.AddModelError("", "Некорректные логин и(или) пароль");
        }

        return View(model);
    }
    
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Book");
    }
    
    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            Employee user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
            if (user == null)
            {
                _context.Users.Add(new Employee {FirstName = model.FirstName,LastName = model.LastName, Email = model.Email,PhoneNumber = model.PhoneNumber, Password = model.Password });
                await _context.SaveChangesAsync();
                await AuthenticateAsync(model.Email);
                return RedirectToAction("Index", "Book");
            }
            ModelState.AddModelError("", "Некорректные логин и(или) пароль");
        }
        return View(model);
    }
    
    
    private async Task AuthenticateAsync(string userName)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimsIdentity.DefaultNameClaimType, userName)
        };
        ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie",
            ClaimsIdentity.DefaultNameClaimType,
            ClaimsIdentity.DefaultRoleClaimType);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(id),
            new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTime.UtcNow.AddMinutes(1)
            }
        );
    }
    

    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }
}