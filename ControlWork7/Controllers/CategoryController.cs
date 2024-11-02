using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ControlWork7.Models;

namespace ControlWork7.Controllers
{
    public class CategoryController : Controller
    {
        private readonly LibraryContext _context;

        public CategoryController(LibraryContext context)
        {
            _context = context;
        }
        
        public async Task<IActionResult> Index()
        {
            List<Category> categories = _context.Categories.ToList();
            return View(categories);
        }
        
        public IActionResult Create()
        {
            return View();
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name")] Category category)
        {
            string errorMessage = null;
            if (_context.Categories.Any(c => c.Name == category.Name))
            {
                errorMessage = $"{category.Name} категория с таким именем уже существует!";
                ViewBag.ErrorMessage = errorMessage;
                return View(category);
            }
            
            if (ModelState.IsValid)
            {
                await _context.AddAsync(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }
        
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
