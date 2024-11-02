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
using Microsoft.AspNetCore.Authorization;

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
        public async Task<IActionResult> Index(int? categoryId, string? bookName, SortBookState? sortOrder = SortBookState.NameAsc, int page = 1)
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
            
            if (categoryId.HasValue && categoryId.Value != 0)
                books = books.Where(p => p.CategoryId == categoryId);
            
            if (bookName != null)
                books = books.Where(t => t.Name.Contains(bookName));
            
            int pageSize = 2;
            var items = books.Skip((page - 1) * pageSize).Take(pageSize);
            PageViewModel pvm = new PageViewModel(books.Count(), page, pageSize);
            
            List<Category> categories = await _context.Categories.ToListAsync();
            categories.Insert(0, new Category(){Id = 0, Name = "All categories"});
            ViewBag.Categories = categories;
            
            var vm = new BookModels()
            {
                Book = items.ToList(),
                PageViewModel = pvm,
                Categories = categories
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
            ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name");
            
            return View();
        }

        //---------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Author,CoverImageUrl,YearPublished,Description, CategoryId")] Book book)
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

        //--------------------------------------------
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> LoanBook(int id)
        {
            Book book = await _context.Books.FindAsync(id);
            if (book == null || book.Status == Status.Выдана)
            {
                ModelState.AddModelError("", "Книга недоступна для выдачи");
                return RedirectToAction("Index");
            }

            string email = User.Identity.Name; 
            Employee user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Пользователь не найден");
                return RedirectToAction("Index");
            }

            int count = await _context.BookLoans.CountAsync(b => b.UserId == user.Id && b.ReturnDate == null);
            if (count >= 3)
            {
                ModelState.AddModelError(string.Empty, "Вы не можете взять больше 3 книг");
                return RedirectToAction("Index");
            }

            BookLoan bookLoan = new BookLoan
            {
                UserId = user.Id,
                BookId = book.Id,
                LoanDate = DateOnly.FromDateTime(DateTime.UtcNow)
            };

            book.Status = Status.Выдана;
            _context.Books.Update(book);
            _context.BookLoans.Add(bookLoan);
            try
            {
                await _context.SaveChangesAsync(); 
                return RedirectToAction("PersonalAccount"); 
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError("", "Error");
                return RedirectToAction("Index");
            }
        }
        
        public async Task<IActionResult> PersonalAccount()
        {
            string email = User.Identity.Name;
            Employee user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Пользователь с таким Email не найден");
                return View(new List<BookLoan>());
            }

            var loans = await _context.BookLoans.Where(b => b.UserId == user.Id && b.ReturnDate == null).Include(b => b.Book).ToListAsync();
            return View(loans);
        }
        
        
        [HttpPost]
        public async Task<IActionResult> ReturnBook(int loanId)
        {
            BookLoan l = await _context.BookLoans.FindAsync(loanId);
            if (l == null)
            {
                ModelState.AddModelError(string.Empty, "Запись о выдаче не найдена");
                return RedirectToAction("PersonalAccount");
            }
            l.ReturnDate = DateOnly.FromDateTime(DateTime.UtcNow);
            Book book = await _context.Books.FindAsync(l.BookId);
            book.Status = Status.В_наличии;
            _context.Books.Update(book);
            await _context.SaveChangesAsync();
            return RedirectToAction("PersonalAccount", new { email = User.Identity.Name });
        }
    }
}
