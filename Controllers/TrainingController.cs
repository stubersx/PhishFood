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
    public class TrainingController : Controller
    {
        private readonly PhishFoodContext _context;

        public TrainingController(PhishFoodContext context)
        {
            _context = context;
        }

        // Training/Train
        public async Task<IActionResult> Train(int? categoryId, int? subcategoryId)
        {
            // Start by including the Category to filter by Category.Name
            var trainings = _context.Trainings.Include(t => t.Category).Include(t => t.Subcategory).AsQueryable();

            if (categoryId.HasValue)
            {
                trainings = trainings.Where(t => t.CategoryID == categoryId.Value);
            }

            if (subcategoryId.HasValue)
            {
                trainings = trainings.Where(t => t.SubcategoryID == subcategoryId.Value);
            }

            ViewBag.Categories = new SelectList(_context.Categories.OrderBy(c => c.Type), "ID", "Type");
            ViewBag.Subcategories = new SelectList(_context.Subcategories.OrderBy(s => s.Type), "ID", "Type");
            ViewBag.Count = trainings.Count();

            return View(trainings.ToList());  // Return the filtered Testings to the View
        }
        [Authorize(Roles = "Admin")]
        // GET: Training
        public async Task<IActionResult> Index(string sort, string searchQuery)
        {
            ViewData["Category"] = String.IsNullOrEmpty(sort) ? "cat_desc" : "";
            ViewData["Subcategory"] = sort == "sub" ? "sub_desc" : "sub";

            var training = from t in _context.Trainings.Include(t => t.Category).Include(t => t.Subcategory)
                           select t;

            if (!string.IsNullOrEmpty(searchQuery))
            {
                training = training.Where(t => t.Content.ToLower().Contains(searchQuery)
                                                || t.Category.Type.ToLower().Contains(searchQuery)
                                                || t.Subcategory.Type.ToLower().Contains(searchQuery));
            }

            switch (sort)
            {
                case "cat_desc":
                    training = training.OrderByDescending(t => t.Category.Type);
                    break;
                case "sub":
                    training = training.OrderBy(t => t.Subcategory.Type);
                    break;
                case "sub_desc":
                    training = training.OrderByDescending(t => t.Subcategory.Type);
                    break;
                default:
                    training = training.OrderBy(t => t.Category.Type);
                    break;
            }

            return View(await training.ToListAsync());
        }
        [Authorize(Roles = "Admin")]
        // GET: Training/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var training = await _context.Trainings
                .Include(t => t.Category)
                .Include(t => t.Subcategory)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (training == null)
            {
                return NotFound();
            }

            return View(training);
        }
        [Authorize(Roles = "Admin")]
        // GET: Training/Create
        public IActionResult Create()
        {
            ViewData["CategoryID"] = new SelectList(_context.Categories, "ID", "Type");
            ViewData["SubcategoryID"] = new SelectList(_context.Subcategories, "ID", "Type");
            return View();
        }

        // POST: Training/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name,Content,Notes,CategoryID,SubcategoryID")] Training training)
        {
            if (ModelState.IsValid)
            {
                _context.Add(training);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryID"] = new SelectList(_context.Categories, "ID", "Type", training.CategoryID);
            ViewData["SubcategoryID"] = new SelectList(_context.Subcategories, "ID", "Type", training.SubcategoryID);
            return View(training);
        }

        // GET: Training/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var training = await _context.Trainings.FindAsync(id);
            if (training == null)
            {
                return NotFound();
            }
            ViewData["CategoryID"] = new SelectList(_context.Categories, "ID", "Type", training.CategoryID);
            ViewData["SubcategoryID"] = new SelectList(_context.Subcategories, "ID", "Type", training.SubcategoryID);
            return View(training);
        }

        // POST: Training/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name,Content,Notes,CategoryID,SubcategoryID")] Training training)
        {
            if (id != training.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(training);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TrainingExists(training.ID))
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
            ViewData["CategoryID"] = new SelectList(_context.Categories, "ID", "Type", training.CategoryID);
            ViewData["SubcategoryID"] = new SelectList(_context.Subcategories, "ID", "Type", training.SubcategoryID);
            return View(training);
        }

        // GET: Training/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var training = await _context.Trainings
                .Include(t => t.Category)
                .Include(t => t.Subcategory)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (training == null)
            {
                return NotFound();
            }

            return View(training);
        }

        // POST: Training/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var training = await _context.Trainings.FindAsync(id);
            if (training != null)
            {
                _context.Trainings.Remove(training);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TrainingExists(int id)
        {
            return _context.Trainings.Any(e => e.ID == id);
        }
    }
}
