using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ControlWork7.Models;
using ControlWork7.Services;
using ControlWork7.ViewModels;

namespace ControlWork7.Controllers
{
    public class BookController : Controller
    {
        private readonly LibraryContext _context;

        public BookController(LibraryContext context)
        {
            _context = context;
        }

        
        //---------------------------------------------
        public async Task<IActionResult> Index(SortBookState? sortOrder = SortBookState.NameAsc, int page = 1)
        {
            IEnumerable<Book> books = await _context.Books.ToListAsync();
            ViewBag.NameSort = sortOrder == SortBookState.NameAsc ? SortBookState.NameDesc : SortBookState.NameAsc;
            ViewBag.AuthorSort = sortOrder == SortBookState.AuthorAsc ? SortBookState.AuthorDesc : SortBookState.AuthorAsc;
            ViewBag.StatusSort = sortOrder == SortBookState.StatusAsc ? SortBookState.StatusDesc : SortBookState.StatusAsc;

            switch (sortOrder)
            {
                case SortBookState.NameAsc:
                    books = books.OrderBy(b => b.Name);
                    break;
                case SortBookState.NameDesc:
                    books = books.OrderByDescending(b => b.Name);
                    break;
                
                case SortBookState.AuthorAsc:
                    books = books.OrderBy(b => b.Author);
                    break;
                case SortBookState.AuthorDesc:
                    books = books.OrderByDescending(b => b.Author);
                    break;
                
                case SortBookState.StatusAsc:
                    books = books.OrderBy(b => b.Status);
                    break;
                case SortBookState.StatusDesc:
                    books = books.OrderByDescending(b => b.Status);
                    break;
            }
            
            int pageSize = 2;
            var items = books.Skip((page - 1) * pageSize).Take(pageSize);
            PageViewModel pvm = new PageViewModel(books.Count(), page, pageSize);
            
            var vm = new BookModels()
            {
                Book = items.ToList(),
                PageViewModel = pvm
            };
            
            return View(vm);
        }
        //---------------------------------------------
      
        
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }
        
        public IActionResult Create()
        {
            return View();
        }

        //---------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Author,CoverImageUrl,YearPublished,Description")] Book book)
        {
            if (ModelState.IsValid)
            {
                book.Status = Status.В_наличии;
                book.CreatedDate = DateOnly.FromDateTime(DateTime.UtcNow);
                
                await _context.AddAsync(book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(book);
        }
        //---------------------------------------------
        
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            return View(book);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Book book)
        {
            if (id != book.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(book);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(book.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(book);
        }

        
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book != null)
            {
                _context.Books.Remove(book);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(int id)
        {
            return _context.Books.Any(e => e.Id == id);
        }
    }
}
