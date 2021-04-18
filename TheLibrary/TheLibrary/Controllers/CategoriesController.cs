using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TheLibrary.Data;
using TheLibrary.Models;

namespace TheLibrary.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly AppDbContext _context;

        public CategoriesController(AppDbContext context)
        {
            _context = context;
        }

        // Get ALl Categories
        public async Task<IActionResult> Index()
        {
            return View(await _context.Categories.ToListAsync());
        }

        //Detail page for Categories
        public async Task<IActionResult> CategoryDetails(int? id)
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

        #region Create Categories
        public IActionResult AddCategory()
        {
            return View();
        }

        // Create Categories
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCategory([Bind("Id,CategoryName")] Category category)
        {
            Category existingCategory = _context.Categories.FirstOrDefault( //getting existing category from Category Model
                c => c.CategoryName == category.CategoryName.ToLower());

            //Check if category already exist, if not create new category
            if (existingCategory != null)
            {
                ModelState.AddModelError(string.Empty, "This Category Already Exist");
                return View(category);
            }
            else if(ModelState.IsValid)
            {
                _context.Add(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }
        #endregion

        #region Edit Categories
        public async Task<IActionResult> EditCategory(int? id)
        {
            if (id == null)
                return NotFound();

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
                return NotFound();
           
            return View(category);
        }

        //Updating Categories
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCategory(int id, [Bind("Id,CategoryName")] Category category)
        {
            bool CategoryExists(int id)
            {
                return _context.Categories.Any(e => e.Id == id);
            }

            if (id != category.Id)
                return NotFound();

            Category existingCategory = _context.Categories.FirstOrDefault( //getting existing category from Category Model
                c => c.CategoryName == category.CategoryName.ToLower());

            //Check if category already exist, if not create new category
            if (existingCategory != null)
            {
                ModelState.AddModelError(string.Empty, "This Category Already Exist");
                return View(category);
            }
            else if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(category);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.Id))                    
                        return NotFound();
                    
                    else                
                        throw;
                    
                }
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }
        #endregion

        #region Delete Categories
        // Delete Categories
        public async Task<IActionResult> RemoveCategory(int? id)
        {
            if (id == null)
                return NotFound();

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
                return NotFound();

            return View(category);
        }

        // Confirm Delete
        [HttpPost, ActionName("RemoveCategory")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            //Getting the Library Item's Count from Library Item Model
            var LibraryItemsCount = _context.LibraryItems.Include(c => c.Category).Count(x => x.CategoryId == id);
            // Check Library count in Category, if category is not null it wont be able to be removed
            if (LibraryItemsCount > 0)
            {
                ModelState.AddModelError(string.Empty, "This Category is not Empty");
                return View();
            }
            else
            {
                var category = await _context.Categories.FindAsync(id);
                //Removing Category from Database
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
        }
        #endregion
        
    }
}
