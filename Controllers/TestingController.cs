using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PhishFood.Models;
using PhishFood;
using static System.Formats.Asn1.AsnWriter;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace PhishFood.Controllers
{
    public class TestingController : Controller
    {
        private readonly PhishFoodContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public TestingController(PhishFoodContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> TestSelection()
        {
            if (!User.Identity.IsAuthenticated)
            {
                // Set an error message and redirect to the login page
                TempData["ErrorMessage"] = "You must log in to take a test.";
                return Redirect("/Identity/Account/Login");
            }

            var user = await _userManager.GetUserAsync(User);
            var student = await _context.Students.FirstOrDefaultAsync(s => s.ID == user.UserName);
            if (student == null)
            {
                // Handle case where student data is not found
                TempData["ErrorMessage"] = "Student record not found.";
                return Redirect("/Identity/Account/Login");
            }
            var completedCategoryIds = await _context.Results
                .Where(r => r.StudentID == student.ID)
                .Select(r => r.CategoryID)
                .Distinct()
                .ToListAsync();

            var testings = await _context.Testings
                .Include(t => t.Category)
                .Include(t => t.Subcategory)
                .ToListAsync();

            // Unique categories
            var categories = testings
                .GroupBy(t => new { t.CategoryID, Category = t.Category.Type })
                .Select(g => new
                {
                    CategoryID = g.Key.CategoryID,
                    Category = g.Key.Category
                })
                .Distinct()
                .ToList();

            // Unique (Category, Subcategory) pairs
            var subcategories = testings
                .Where(t => t.SubcategoryID != null)
                .GroupBy(t => new { t.CategoryID, t.SubcategoryID, Category = t.Category.Type, Subcategory = t.Subcategory.Type })
                .Select(g => new
                {
                    CategoryID = g.Key.CategoryID,
                    SubcategoryID = g.Key.SubcategoryID,
                    Category = g.Key.Category,
                    Subcategory = g.Key.Subcategory
                })
                .Distinct()
                .ToList();

            ViewBag.Categories = categories;
            ViewBag.Subcategories = subcategories;
            ViewBag.CompletedUnitTests = completedCategoryIds;

            return View();
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
        [HttpGet]
        public async Task<IActionResult> StartTest(int? categoryId, int? subcategoryId)
        {

            var query = _context.Testings
                .Include(t => t.Category)
                .Include(t => t.Subcategory)
                .AsQueryable();

            if (subcategoryId.HasValue)
                query = query.Where(t => t.SubcategoryID == subcategoryId);
            else if (categoryId.HasValue)
                query = query.Where(t => t.CategoryID == categoryId);

            var questions = await query.ToListAsync();
            var selectedQuestions = questions
                .OrderBy(q => Guid.NewGuid())
                .Take(10)
                .Select(q => new QuestionDTO
                {
                    ID = q.ID,
                    Question = q.Question,
                    Key = q.Key,
                    Option1 = q.Option1,
                    Option2 = q.Option2,
                    Option3 = q.Option3,
                    Explanation = q.Explanation
                }).ToList();

            var viewModel = new TestSessionViewModel
            {
                Questions = selectedQuestions,
                CurrentIndex = 0,
                CategoryID = categoryId ?? 0,
                SubcategoryID = subcategoryId
            };

            TempData.Put("TestSession", viewModel);
            return RedirectToAction("Question");
        }
        [HttpGet]
        public IActionResult Question()
        {
            var session = TempData.Get<TestSessionViewModel>("TestSession");

            if (session == null)
                return RedirectToAction("StartTest");

            // Edge case: all questions answered, redirect to final score
            if (session.CurrentIndex >= session.Questions.Count)
                return RedirectToAction("FinalScore");

            var current = session.Questions[session.CurrentIndex];

            // Shuffle the options
            var options = new List<string> { current.Key, current.Option1, current.Option2, current.Option3 };
            var rng = new Random();
            current.ShuffledOptions = options.OrderBy(x => rng.Next()).ToList();

            TempData.Put("TestSession", session);
            return View(session);
        }

        [HttpPost]
        public IActionResult SubmitAnswer(string selectedAnswer)
        {
            var session = TempData.Get<TestSessionViewModel>("TestSession");
            if (session == null)
                return RedirectToAction("StartTest");

            var current = session.Questions[session.CurrentIndex];
            session.SelectedAnswer = selectedAnswer;
            session.IsCorrect = selectedAnswer == current.Key;

            if (session.IsCorrect)
                session.Score++;

            session.ShowExplanation = true;

            TempData.Put("TestSession", session);
            return RedirectToAction("Question");
        }
        [HttpPost]
        public IActionResult NextQuestion()
        {
            var session = TempData.Get<TestSessionViewModel>("TestSession");
            if (session == null)
                return RedirectToAction("StartTest");

            session.CurrentIndex++;
            session.ShowExplanation = false;
            session.IsCorrect = false;

            TempData.Put("TestSession", session);

            return RedirectToAction("Question");
        }
        public async Task<IActionResult> FinalScore()
        {
            var session = TempData.Get<TestSessionViewModel>("TestSession");
            if (session == null)
                return RedirectToAction("StartTest");

            // Capture the username (assumed to match StudentID)
            var studentId = User.Identity?.Name;
            if (string.IsNullOrEmpty(studentId))
            {
                // Redirect or handle case where user is not authenticated
                return Redirect("/Identity/Account/Login");
            }

            // Don't save subcategory-level results
            if (session.SubcategoryID == null)
            {
                var category = await _context.Categories.FindAsync(session.CategoryID);
                if (category != null)
                {
                    var result = new Result
                    {
                        Date = DateTime.Now,
                        Score = session.Score,
                        CategoryID = category.ID,
                        StudentID = studentId
                    };

                    _context.Results.Add(result);
                    await _context.SaveChangesAsync();
                }
            }

            return View(session);
        }
        [Authorize(Roles ="Admin")]
        // GET: Testing
        public async Task<IActionResult> Index(string searchQuery)
        {
            // Start by including the Category to filter by Category.Name
            var testingsQuery = _context.Testings.Include(t => t.Category).Include(t => t.Subcategory).AsQueryable();

            // If searchQuery is provided, filter Testings by Question text and Category name
            if (!string.IsNullOrEmpty(searchQuery))
            {
                var normalizedSearchQuery = searchQuery.Trim().ToLower();  // Make search case-insensitive

                testingsQuery = testingsQuery.Where(t =>
                    t.Question.ToLower().Contains(normalizedSearchQuery) ||  // Search in Question Text
                    t.Category.Type.ToLower().Contains(normalizedSearchQuery) ||
                    t.Subcategory.Type.ToLower().Contains(normalizedSearchQuery)// Search in Category Name
                );
            }

            // Get the list of filtered Testings asynchronously
            var testings = await testingsQuery.ToListAsync();

            return View(testings);  // Return the filtered Testings to the View
        }
        [Authorize(Roles = "Admin")]
        // GET: Testing/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var testing = await _context.Testings
                .Include(t => t.Category)
                .Include(t => t.Subcategory)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (testing == null)
            {
                return NotFound();
            }

            return View(testing);
        }
        [Authorize(Roles = "Admin")]
        // GET: Testing/Create
        public IActionResult Create()
        {
            ViewData["CategoryID"] = new SelectList(_context.Categories, "ID", "Type");
            ViewData["SubcategoryID"] = new SelectList(_context.Subcategories, "ID", "Type");
            return View();
        }

        // POST: Testing/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("ID,Question,Key,Option1,Option2,Option3,Explanation,CategoryID,SubcategoryID")] Testing testing)
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
            ViewData["SubcategoryID"] = new SelectList(_context.Subcategories, "ID", "Type", testing.SubcategoryID);
            return View(testing);
        }

        // GET: Testing/Edit/5
        [Authorize(Roles = "Admin")]
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
            ViewData["SubcategoryID"] = new SelectList(_context.Subcategories, "ID", "Type", testing.SubcategoryID);
            return View(testing);
        }

        // POST: Testing/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Question,Key,Option1,Option2,Option3,Explanation,CategoryID,SubcategoryID")] Testing testing)
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
            ViewData["SubcategoryID"] = new SelectList(_context.Subcategories, "ID", "Type", testing.SubcategoryID);
            return View(testing);
        }

        // GET: Testing/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var testing = await _context.Testings
                .Include(t => t.Category)
                .Include(t => t.Subcategory)
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
        [Authorize(Roles = "Admin")]
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
