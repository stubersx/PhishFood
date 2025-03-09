using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PhishFood.Models;

namespace PhishFood.Controllers
{
    public class TestingController : Controller
    {
        private readonly PhishFoodContext _context;

        public TestingController(PhishFoodContext context)
        {
            _context = context;
        }

        // GET: Testing
        public async Task<IActionResult> Index(string searchQuery)
        {
            // Start by including the Category to filter by Category.Name
            var testingsQuery = _context.Testings.Include(t => t.Category).Include(t => t.SubCategory).AsQueryable();

            // If searchQuery is provided, filter Testings by Question text and Category name
            if (!string.IsNullOrEmpty(searchQuery))
            {
                var normalizedSearchQuery = searchQuery.Trim().ToLower();  // Make search case-insensitive

                testingsQuery = testingsQuery.Where(t =>
                    t.Question.ToLower().Contains(normalizedSearchQuery) ||  // Search in Question Text
                    t.Category.Type.ToLower().Contains(normalizedSearchQuery) ||
                    t.SubCategory.Type.ToLower().Contains(normalizedSearchQuery)// Search in Category Name
                );
            }

            // Get the list of filtered Testings asynchronously
            var testings = await testingsQuery.ToListAsync();

            return View(testings);  // Return the filtered Testings to the View
        }

        // GET: Testing/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var testing = await _context.Testings
                .Include(t => t.Category)
                .Include(t => t.SubCategory)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (testing == null)
            {
                return NotFound();
            }

            return View(testing);
        }

        // GET: Testing/Create
        public IActionResult Create()
        {
            ViewData["CategoryID"] = new SelectList(_context.Categories, "ID", "Type");
            ViewData["SubCategoryID"] = new SelectList(_context.SubCategories, "ID", "Type");
            return View();
        }

        // POST: Testing/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Question,Key,Option1,Option2,Option3,Explanation,CategoryID,SubCategoryID")] Testing testing)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(testing);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, $"An error occurred while saving: {ex.Message}");
                }
            }
            ViewData["CategoryID"] = new SelectList(_context.Categories, "ID", "Type", testing.CategoryID);
            ViewData["SubCategoryID"] = new SelectList(_context.SubCategories, "ID", "Type", testing.SubCategoryID);
            return View(testing);
        }

        // GET: Testing/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var testing = await _context.Testings.FindAsync(id);
            if (testing == null)
            {
                return NotFound();
            }
            ViewData["CategoryID"] = new SelectList(_context.Categories, "ID", "Type", testing.CategoryID);
            ViewData["SubCategoryID"] = new SelectList(_context.SubCategories, "ID", "Type", testing.SubCategoryID);
            return View(testing);
        }

        // POST: Testing/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Question,Key,Option1,Option2,Option3,Explanation,CategoryID,SubCategoryID")] Testing testing)
        {
            if (id != testing.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(testing);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TestingExists(testing.ID))
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
            ViewData["CategoryID"] = new SelectList(_context.Categories, "ID", "Type", testing.CategoryID);
            ViewData["SubCategoryID"] = new SelectList(_context.SubCategories, "ID", "Type", testing.SubCategoryID);
            return View(testing);
        }

        // GET: Testing/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var testing = await _context.Testings
                .Include(t => t.Category)
                .Include(t => t.SubCategory)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (testing == null)
            {
                return NotFound();
            }

            return View(testing);
        }

        // POST: Testing/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var testing = await _context.Testings.FindAsync(id);
            if (testing != null)
            {
                _context.Testings.Remove(testing);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TestingExists(int id)
        {
            return _context.Testings.Any(e => e.ID == id);
        }
    }
}
