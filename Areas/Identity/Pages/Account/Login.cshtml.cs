using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using PhishFood.Models;
using Microsoft.EntityFrameworkCore;

namespace PhishFood.Areas.Identity.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;
        private readonly PhishFoodContext _context;

        public LoginModel(SignInManager<ApplicationUser> signInManager, ILogger<LoginModel> logger, PhishFoodContext context)
        {
            _signInManager = signInManager;
            _logger = logger;
            _context = context;
        }

        // Bind the input model to capture form inputs
        [BindProperty]
        public InputModel Input { get; set; }

        // List of external login schemes
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        // Return URL
        public string ReturnUrl { get; set; }

        // Temp data for error messages
        [TempData]
        public string ErrorMessage { get; set; }

        // Input model for login data
        public class InputModel
        {
            [Required]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        // Get method for the login page
        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            ReturnUrl = returnUrl;
        }

        // Ensure student exists or create a new student if not
        private async Task EnsureStudentExists(ApplicationUser user)
        {
            // Skip if the user is an admin
            var isAdmin = await _signInManager.UserManager.IsInRoleAsync(user, "Admin");
            if (isAdmin) return;

            // Check if the student already exists
            var exists = await _context.Students.AnyAsync(s => s.ID == user.UserName);
            if (exists) return;

            // Create new student record
            var student = new Student
            {
                ID = user.UserName,
                FirstName = "Unknown",
                LastName = "Unknown"
            };

            _context.Students.Add(student);
            await _context.SaveChangesAsync();
        }

        // Post method for handling login submission
        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                ApplicationUser user = await _signInManager.UserManager.FindByEmailAsync(Input.Email);

                // If no user found by email, try finding by username
                if (user == null)
                {
                    user = await _signInManager.UserManager.FindByNameAsync(Input.Email);
                }

                // If user is found, attempt to sign them in
                if (user != null)
                {
                    var result = await _signInManager.PasswordSignInAsync(user, Input.Password, Input.RememberMe, lockoutOnFailure: false);

                    if (result.Succeeded)
                    {
                        _logger.LogInformation("User logged in.");

                        await EnsureStudentExists(user);

                        return LocalRedirect(returnUrl);
                    }

                    if (result.RequiresTwoFactor)
                    {
                        return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                    }

                    if (result.IsLockedOut)
                    {
                        _logger.LogWarning("User account locked out.");
                        return RedirectToPage("./Lockout");
                    }

                    // If login failed
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                }
                else
                {
                    // If no user is found with the given email/username
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                }
            }

            // If we got here, something failed, redisplay form
            return Page();
        }
    }
}
