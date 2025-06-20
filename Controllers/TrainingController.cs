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
            IQueryable<Training> trainings = _context.Trainings
                .Include(t => t.Category)
                .Include(t => t.Subcategory)
                .Where(t => t.IsActive);

            int count = 0;

            if (categoryId.HasValue && subcategoryId.HasValue)
            {
                trainings = trainings
                    .Include(t => t.Category)
                    .Include(t => t.Subcategory)
                    .Where(t => t.CategoryID == categoryId.Value && t.SubcategoryID == subcategoryId.Value);

                count = await trainings.CountAsync();
            }

            var trainingsList = await trainings.ToListAsync();
            ViewBag.Count = count;

            ViewBag.Categories = new SelectList(_context.Categories.OrderBy(c => c.Type), "ID", "Type", categoryId);

            if (categoryId.HasValue)
            {
                ViewBag.Subcategories = new SelectList(
                    _context.Subcategories
                        .Where(s => s.CategoryID == categoryId.Value)
                        .OrderBy(s => s.Type),
                    "ID",
                    "Type",
                    subcategoryId);
            }
            else
            {
                ViewBag.Subcategories = new SelectList(Enumerable.Empty<SelectListItem>());
            }

            return View(trainingsList);
        }

        [Authorize(Roles = "Admin")]
        // GET: Training
        public async Task<IActionResult> Index(string sort, string searchQuery, bool? showInactive = false)
        {

            var training = from t in _context.Trainings.Include(t => t.Category).Include(t => t.Subcategory)
                           select t;

            if (showInactive != true)
            {
                training = training.Where(t => t.IsActive);
            }

            if (!string.IsNullOrEmpty(searchQuery))
            {
                var query = searchQuery.ToLower();
                training = training.Where(t =>
                    t.Name.ToLower().Contains(query) ||
                    t.Content.ToLower().Contains(query) ||
                    t.Category.Type.ToLower().Contains(query) ||
                    t.Subcategory.Type.ToLower().Contains(query));
            }

            switch (sort)
            {
                case "cat_asc":
                    training = training.OrderBy(t => t.Category.Type);
                    break;
                case "cat_desc":
                    training = training.OrderByDescending(t => t.Category.Type);
                    break;
                case "sub_asc":
                    training = training.OrderBy(t => t.Subcategory.Type);
                    break;
                case "sub_desc":
                    training = training.OrderByDescending(t => t.Subcategory.Type);
                    break;
                case "name_asc":
                    training = training.OrderBy(t => t.Name);
                    break;
                case "name_desc":
                    training = training.OrderByDescending(t => t.Name);
                    break;
                case "active_asc":
                    training = training.OrderBy(t => t.IsActive);
                    break;
                case "active_desc":
                    training = training.OrderByDescending(t => t.IsActive);
                    break;
                default:
                    training = training.OrderBy(t => t.ID);
                    break;
            }

            ViewData["SortOrder"] = sort;
            ViewData["searchQuery"] = searchQuery;
            ViewData["IsActiveSort"] = sort == "active_asc" ? "active_desc" : "active_asc";
            ViewBag.ShowInactive = showInactive;

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
        public async Task<IActionResult> Create([Bind("ID,Name,Content,Notes,CategoryID,SubcategoryID, IsActive")] Training training)
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
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name,Content,Notes,CategoryID,SubcategoryID, IsActive")] Training training)
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

        [HttpGet]
        public JsonResult GetSubcategories(int categoryId)
        {
            var subcategories = _context.Subcategories
                .Where(s => s.CategoryID == categoryId)
                .OrderBy(s => s.Type)
                .Select(s => new { s.ID, s.Type })
                .ToList();

            return Json(subcategories);
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
