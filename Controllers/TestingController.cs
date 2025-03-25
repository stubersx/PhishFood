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

        // Testing/TakeTest
        public async Task<IActionResult> TakeTest()
        {
            var tests = await _context.Testings
                .Include(t => t.Category)
                .Include(t => t.Subcategory)
                .ToListAsync();

            var categories = tests.Select(t => t.Category.Type).Distinct().ToList();
            var subcategories = tests
                .Where(t => t.Subcategory != null)
                .Select(t => t.Subcategory.Type)
                .Distinct()
                .ToList();

            ViewData["Tests"] = tests;
            ViewData["Categories"] = categories;
            ViewData["Subcategories"] = subcategories;

            return View();
        }

        public async Task<IActionResult> StartTest(string category, string subcategory = null)
        {
            var query = _context.Testings.Include(t => t.Category).Include(t => t.Subcategory).AsQueryable();

            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(t => t.Category.Type == category);
            }

            if (!string.IsNullOrEmpty(subcategory))
            {
                query = query.Where(t => t.Subcategory.Type == subcategory);
            }

            var randomQuestions = await query.OrderBy(r => Guid.NewGuid()).Take(10).ToListAsync();

            if (randomQuestions.Count == 0)
            {
                return NotFound("No test questions found for the selected category.");
            }

            // Shuffle the options for each question
            foreach (var question in randomQuestions)
            {
                var options = new List<string> { question.Key, question.Option1, question.Option2, question.Option3 };
                options = options.OrderBy(o => Guid.NewGuid()).ToList();

                question.Key = options[0];
                question.Option1 = options[1];
                question.Option2 = options[2];
                question.Option3 = options[3];
            }

            // Randomize the order of questions
            randomQuestions = randomQuestions.OrderBy(q => Guid.NewGuid()).ToList();

            ViewBag.Category = category;
            ViewBag.Subcategory = subcategory;

            return View("TestView", randomQuestions);
        }

        public async Task<IActionResult> TestView(int? categoryId, int? subcategoryId)
        {
            var tests = await _context.Testings
                .Where(t => (categoryId == null || t.CategoryID == categoryId) && (subcategoryId == null || t.SubcategoryID == subcategoryId))
                .Include(t => t.Category)
                .Include(t => t.Subcategory)
                .OrderBy(t => Guid.NewGuid()) // Randomize question order
                .Take(10) // Take 10 random questions
                .ToListAsync();

            ViewData["Category"] = tests.FirstOrDefault()?.Category?.Type;
            ViewData["Subcategory"] = tests.FirstOrDefault()?.Subcategory?.Type;

            return View(tests);
        }

        // Action to handle test submission and calculate score
        [HttpPost]
        public async Task<IActionResult> SubmitTest(List<TestAnswer> Answers)
        {
            int correctCount = 0;
            foreach (var answer in Answers)
            {
                var question = await _context.Testings.FindAsync(answer.QuestionId);
                if (question != null && question.Key == answer.SelectedOption)
                {
                    correctCount++;
                }
            }

            ViewBag.Score = $"{correctCount} / {Answers.Count}";
            return View("TestResults");
        }
        // Action to display the final score
        public IActionResult TestResults(int score)
        {
            return View(score);
        }

        public IActionResult GradedTestWarning(string category)
        {
            if (string.IsNullOrEmpty(category))
            {
                return RedirectToAction("TakeTest");
            }

            return View("GradedTestWarning", category);
        }

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
