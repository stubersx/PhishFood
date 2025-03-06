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
    public class ResultController : Controller
    {
        private readonly PhishFoodContext _context;

        public ResultController(PhishFoodContext context)
        {
            _context = context;
        }

        // GET: Result
        public async Task<IActionResult> Index()
        {
            var phishFoodContext = _context.Results.Include(r => r.Category).Include(r => r.Student);
            return View(await phishFoodContext.ToListAsync());
        }

        // GET: Result/Details/5
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
