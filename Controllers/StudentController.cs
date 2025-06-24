using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PhishFood.Helpers;
using PhishFood.Models;
using PhishFood.ViewModels;

namespace PhishFood.Controllers
{
    public class StudentController : Controller
    {
        private readonly PhishFoodContext _context;

        public StudentController(PhishFoodContext context)
        {
            _context = context;
        }

        // GET: Student
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index(string sortOrder, string searchQuery, int pageNumber = 1)
        {
            int pageSize = 100;

            var query = _context.Students
                .AsNoTracking()
                .Select(s => new StudentListViewModel
                {
                    ID = s.ID,
                    FirstName = s.FirstName,
                    LastName = s.LastName
                });

            if (!string.IsNullOrEmpty(searchQuery))
            {
                var lower = searchQuery.Trim().ToLower();  // Make search case-insensitive

                query = query.Where(t =>
                    t.FirstName.ToLower().Contains(lower) ||  // Search by First Name
                    t.LastName.ToLower().Contains(lower) ||   // Search by Last Name
                    t.ID.ToString().ToLower().Contains(lower)  // Search by ID (ensure ID is converted to string for the comparison)
                );
            }

            query = sortOrder switch
            {
                "id_desc" => query.OrderByDescending(s => s.ID),
                "FirstName" => query.OrderBy(s => s.FirstName),
                "firstname_desc" => query.OrderByDescending(s => s.FirstName),
                "LastName" => query.OrderBy(s => s.LastName),
                "lastname_desc" => query.OrderByDescending(s => s.LastName),
                _ => query.OrderBy(s => s.ID)
            };

            var paginatedData = await PaginatedList<StudentListViewModel>.CreateAsync(query, pageNumber, pageSize);

            ViewData["CurrentSort"] = sortOrder;
            ViewData["IDSortParm"] = String.IsNullOrEmpty(sortOrder) ? "id_desc" : "";
            ViewData["FirstNameSortParm"] = sortOrder == "FirstName" ? "firstname_desc" : "FirstName";
            ViewData["LastNameSortParm"] = sortOrder == "LastName" ? "lastname_desc" : "LastName";
            ViewData["searchQuery"] = searchQuery;
            ViewBag.Pagination = paginatedData;

            return View(paginatedData);
        }

        // GET: Student/Details/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .FirstOrDefaultAsync(m => m.ID == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // GET: Student/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Student/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("ID,FirstName,LastName")] Student student)
        {
            if (ModelState.IsValid)
            {
                _context.Add(student);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(student);
        }

        // GET: Student/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }
            return View(student);
        }

        // POST: Student/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(string id, [Bind("ID,FirstName,LastName")] Student student)
        {
            if (id != student.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(student);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentExists(student.ID))
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
            return View(student);
        }

        // GET: Student/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .FirstOrDefaultAsync(m => m.ID == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // POST: Student/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student != null)
            {
                _context.Students.Remove(student);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StudentExists(string id)
        {
            return _context.Students.Any(e => e.ID == id);
        }
    }
}
