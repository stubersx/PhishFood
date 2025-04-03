using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace YourProject.Controllers
{
    public class AccountController : Controller
    {
        // Action to initiate Google login
        public IActionResult Login()
        {
            // Redirect to Google login (this triggers the OAuth process)
            return Challenge(new AuthenticationProperties { RedirectUri = "/" }, GoogleDefaults.AuthenticationScheme);
        }

        // Action to handle logout
        public async Task<IActionResult> Logout()
        {
            // Sign out from both Google and cookies
            await HttpContext.SignOutAsync(GoogleDefaults.AuthenticationScheme);
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Redirect to home page or any other page after logout
            return RedirectToAction("Index", "Home");
        }
    }
}
