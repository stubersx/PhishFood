﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        // GET: Training
        public async Task<IActionResult> Index()
        {
            var phishFoodContext = _context.Trainings.Include(t => t.Category).Include(t => t.Subcategory);
            return View(await phishFoodContext.ToListAsync());
        }

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
        public async Task<IActionResult> Create([Bind("ID,Content,CategoryID,SubcategoryID")] Training training)
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
        public async Task<IActionResult> Edit(int id, [Bind("ID,Content,CategoryID,SubcategoryID")] Training training)
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
