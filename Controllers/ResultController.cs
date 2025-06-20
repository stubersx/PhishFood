using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PhishFood.Models;

namespace PhishFood.Controllers
{
    public class ResultController : Controller
    {
        private readonly PhishFoodContext _context;

        public ResultController(PhishFoodContext context)
        {
            _context = context;
        }

        // GET: Result
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index(string searchQuery, string sortOrder)
        {

            ViewData["SortOrder"] = sortOrder;
            ViewData["DateSortParm"] = sortOrder == "date_asc" ? "date_desc" : "date_asc";
            ViewData["ScoreSortParm"] = sortOrder == "score_asc" ? "score_desc" : "score_asc";
            ViewData["CategorySortParm"] = sortOrder == "category_asc" ? "category_desc" : "category_asc";
            ViewData["StudentSortParm"] = sortOrder == "student_asc" ? "student_desc" : "student_asc";
            ViewData["SearchQuery"] = searchQuery;

            // Initialize the base query to get the Results
            var resultsQuery = _context.Results.Include(r => r.Category).Include(r => r.Student).AsQueryable();

            // If searchQuery is provided, filter the results
            if (!string.IsNullOrEmpty(searchQuery))
            {
                if (DateTime.TryParse(searchQuery, out DateTime searchDate))
                {
                    resultsQuery = resultsQuery.Where(r => r.Date.Date == searchDate.Date);
                }
                else
                {
                    var normalizedSearchQuery = searchQuery.Trim().ToLower();
                    resultsQuery = resultsQuery.Where(r =>
                        r.Score.ToString().Contains(normalizedSearchQuery) ||
                        r.Category.Type.ToLower().Contains(normalizedSearchQuery) ||
                        r.StudentID.ToLower().Contains(normalizedSearchQuery));
                }
            }

            // Sorting logic based on the sortOrder parameter
            resultsQuery = sortOrder switch
            {
                "date_desc" => resultsQuery.OrderByDescending(r => r.Date),
                "date_asc" => resultsQuery.OrderBy(r => r.Date),
                "score_desc" => resultsQuery.OrderByDescending(r => r.Score),
                "score_asc" => resultsQuery.OrderBy(r => r.Score),
                "category_desc" => resultsQuery.OrderByDescending(r => r.Category.Type),
                "category_asc" => resultsQuery.OrderBy(r => r.Category.Type),
                "student_desc" => resultsQuery.OrderByDescending(r => r.StudentID),
                "student_asc" => resultsQuery.OrderBy(r => r.StudentID),
                _ => resultsQuery.OrderByDescending(r => r.Date), // default sort
            };

            // Get the list of filtered results asynchronously
            var results = await resultsQuery.ToListAsync();
            return View(results);
        }

        // GET: Result/Details/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var result = await _context.Results
                .Include(r => r.Category)
                .Include(r => r.Student)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (result == null)
            {
                return NotFound();
            }

            return View(result);
        }

        // GET: Result/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["CategoryID"] = new SelectList(_context.Categories, "ID", "Type");
            ViewData["StudentID"] = new SelectList(_context.Students, "ID", "ID");
            return View();
        }

        // POST: Result/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("ID,Date,Score,CategoryID,StudentID")] Result result)
        {
            if (ModelState.IsValid)
            {
                _context.Add(result);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryID"] = new SelectList(_context.Categories, "ID", "Type", result.CategoryID);
            ViewData["StudentID"] = new SelectList(_context.Students, "ID", "ID", result.StudentID);
            return View(result);
        }

        // GET: Result/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var result = await _context.Results.FindAsync(id);
            if (result == null)
            {
                return NotFound();
            }
            ViewData["CategoryID"] = new SelectList(_context.Categories, "ID", "Type", result.CategoryID);
            ViewData["StudentID"] = new SelectList(_context.Students, "ID", "ID", result.StudentID);
            return View(result);
        }

        // POST: Result/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Date,Score,CategoryID,StudentID")] Result result)
        {
            if (id != result.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(result);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ResultExists(result.ID))
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
            ViewData["CategoryID"] = new SelectList(_context.Categories, "ID", "Type", result.CategoryID);
            ViewData["StudentID"] = new SelectList(_context.Students, "ID", "ID", result.StudentID);
            return View(result);
        }

        // GET: Result/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var result = await _context.Results
                .Include(r => r.Category)
                .Include(r => r.Student)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (result == null)
            {
                return NotFound();
            }

            return View(result);
        }

        // POST: Result/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _context.Results.FindAsync(id);
            if (result != null)
            {
                _context.Results.Remove(result);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ResultExists(int id)
        {
            return _context.Results.Any(e => e.ID == id);
        }
    }
}
