using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PhishFood.Models;
using PhishFood.ViewModels;

namespace PhishFood.Controllers
{
    public class SubcategoryController : Controller
    {
        private readonly PhishFoodContext _context;

        public SubcategoryController(PhishFoodContext context)
        {
            _context = context;
        }

        // GET: Subcategory/CategoryView
        public async Task<IActionResult> CategoryView()
        {
            var data = new CategoryViewModel
            {
                Categories = await _context.Categories.ToListAsync(),
                Subcategories = await _context.Subcategories.ToListAsync()
            };

            return View(data);
        }

        // GET: Subcategory/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["CategoryID"] = new SelectList(_context.Categories, "ID", "Type");
            //Category category - ViewData["CategoryID"] = new SelectList(_context.Categories, "ID", "Type", !String.IsNullOrEmpty(category.Type) ? category.ID : "");
            return View();
        }

        // POST: Subcategory/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("ID,Type,CategoryID")] Subcategory subcategory)
        {
            if (ModelState.IsValid)
            {
                foreach (var item in _context.Subcategories)
                {
                    if (item.Type == subcategory.Type)
                        return RedirectToAction("CategoryView");
                }

                _context.Add(subcategory);
                await _context.SaveChangesAsync();
                return RedirectToAction("CategoryView", subcategory);
            }
            ViewData["CategoryID"] = new SelectList(_context.Categories, "ID", "Type", subcategory.CategoryID);
            return View(subcategory);
        }

        // GET: Subcategory/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subcategory = await _context.Subcategories.FindAsync(id);
            if (subcategory == null)
            {
                return NotFound();
            }
            ViewData["CategoryID"] = new SelectList(_context.Categories, "ID", "Type", subcategory.CategoryID);
            return View(subcategory);
        }

        // POST: Subcategory/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Type,CategoryID")] Subcategory subcategory)
        {
            if (id != subcategory.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(subcategory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SubcategoryExists(subcategory.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("CategoryView", "Subcategory");
            }
            ViewData["CategoryID"] = new SelectList(_context.Categories, "ID", "Type", subcategory.CategoryID);
            return View(subcategory);
        }

        // GET: Subcategory/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subcategory = await _context.Subcategories
                .Include(s => s.Category)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (subcategory == null)
            {
                return NotFound();
            }

            return View(subcategory);
        }

        // POST: Subcategory/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var subcategory = await _context.Subcategories.FindAsync(id);
            if (subcategory != null)
            {
                _context.Subcategories.Remove(subcategory);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("CategoryView", "Subcategory");
        }

        private bool SubcategoryExists(int id)
        {
            return _context.Subcategories.Any(e => e.ID == id);
        }
    }
}
