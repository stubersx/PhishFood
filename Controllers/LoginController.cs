using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using Microsoft.AspNetCore.Identity;

namespace YourProject.Controllers
{
    public class LoginController : Controller
    {
        // Action to initiate Google login
        public IActionResult Index()
        {
            return View();
        }

        public async Task Login()
        {
            await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme, 
                new AuthenticationProperties
                {
                    RedirectUri = Url.Action("GoogleResponse")
                });
        }

        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            var claims = result.Principal.Identities.FirstOrDefault().Claims.Select(claim => new
            {
                claim.Issuer,
                claim.OriginalIssuer,
                claim.Type,
                claim.Value
            });
            //return Json(claims);
            return RedirectToAction("Index", "Home", new { area = "" });
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return View("Index");
        }
        public static async Task CreateRoles(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            string[] roles = { "Admin", "Student" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }
    }
}
