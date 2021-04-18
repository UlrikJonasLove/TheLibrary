using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TheLibrary.Data;
using TheLibrary.Models;
using TheLibrary.ViewModels;

namespace TheLibrary.Controllers
{
    public class LibraryItemsController : Controller
    {
        private readonly AppDbContext _context;

        public LibraryItemsController(AppDbContext context)
        {
            _context = context;
        }

        // Get All Library Items
        public IActionResult Index(string sortOrder, string filterString)
        {
            //These lines of code creates the filtering for all the types
            ViewData["TypeSortParm"] = String.IsNullOrEmpty(sortOrder) ? "types" : "";
            ViewData["CurrentFilter"] = filterString;
            var types = from t in _context.LibraryItems
                        select t;
            if (!String.IsNullOrEmpty(filterString))
                types = types.Where(t => t.Type.Contains(filterString));

            switch (sortOrder)
            {
                case "types":
                    types = types.OrderByDescending(t => t.Type);
                    break;
                default:
                    types = types.OrderBy(c => c.Category);
                    break;
            }

            var libs = _context.LibraryItems.Include(c => c.Category).ToList();
            if (filterString == null)
                filterString = GetSession("Filter"); // Get Session
            else
                SetSession("Filter", filterString); // Set Session

            var viewModel = new CategoriesAndLibraryViewModel
            {
                Categories = _context.Categories.ToList(),
                LibraryItems = (filterString != null) ? libs.Where(t => t.Type == filterString).ToList() : libs
            };

            return View(viewModel);
        }

        // Library Item Details
        public async Task<IActionResult> ItemDetails(int? id)
        {
            if (id == null)
                return NotFound();

            var libraryItem = await _context.LibraryItems
                .Include(l => l.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (libraryItem == null)
                return NotFound();

            return View(libraryItem);
        }

        #region Add Library Item
        public IActionResult AddItem()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "CategoryName");
            return View();
        }

        // Add Library Item
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddItem([Bind("Id,CategoryId,Title,Author,Pages,RunTimeMinutes,IsBorrowable,Borrower,Date,Type")] LibraryItem libraryItem)
        {
            if (ModelState.IsValid)
            {
                _context.Add(libraryItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "CategoryName", libraryItem.CategoryId);
            return View(libraryItem);
        }
        #endregion

        #region Edit Library Item
        public async Task<IActionResult> EditItem(int? id)
        {
            if (id == null)
                return NotFound();

            var libraryItem = await _context.LibraryItems.FindAsync(id);
            if (libraryItem == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "CategoryName", libraryItem.CategoryId);
            return View(libraryItem);
        }

       // Edit Library Item
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditItem(int id, [Bind("Id,CategoryId,Title,Author,Pages,RunTimeMinutes,IsBorrowable,Borrower,Date,Type")] LibraryItem libraryItem)
        {
            if (id != libraryItem.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                bool LibraryItemExists(int id)
                {
                    return _context.LibraryItems.Any(e => e.Id == id);
                }

                try
                {
                    _context.Update(libraryItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LibraryItemExists(libraryItem.Id))
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
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "CategoryName", libraryItem.CategoryId);
            return View(libraryItem);
        }
        #endregion

        #region Removing Library Item
        // Removing Library Item
        public async Task<IActionResult> RemoveItem(int? id)
        {
            if (id == null)
                return NotFound();

            var libraryItem = await _context.LibraryItems
                .Include(l => l.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (libraryItem == null)
                return NotFound();

            return View(libraryItem);
        }

        // Confirm Delete
        [HttpPost, ActionName("RemoveItem")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var libraryItem = await _context.LibraryItems.FindAsync(id);
            _context.LibraryItems.Remove(libraryItem);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region Handle Session
        //Get Session
        private string GetSession(string key)
        {
            var value = HttpContext.Session.GetString(key);
            if (value != "undefined" && value != null)
                return value;
            return null;
        }
        //Set Session
        private void SetSession(string key, string value)
        {
            HttpContext.Session.SetString(key, value);
        }
        #endregion
    }
}
