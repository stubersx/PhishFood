using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Cookies;
using PhishFood.Data;
using PhishFood.Models;
using Microsoft.Extensions.Options;
public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var config = builder.Configuration;

        // Add services to the container.
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));
        builder.Services.AddDbContext<PhishFoodContext>(options =>
            options.UseSqlServer(connectionString));

        builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>();



        builder.Services.AddAuthentication()
            .AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
            {
                options.ClientId = config["GoogleKeys:ClientId"];
                options.ClientSecret = config["GoogleKeys:ClientSecret"];
                options.CallbackPath = "/signin-google";
            });

        builder.Services.AddControllersWithViews();
        builder.Services.AddRazorPages();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseMigrationsEndPoint();
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");
        app.MapRazorPages();

        using (var scope = app.Services.CreateScope())
        {
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            var roles = new[] { "Admin", "User" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
        // Seeding admins on startup
        using (var scope = app.Services.CreateScope()) //defualt admin
        {
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            string email = "admin@admin.com";
            string password = "YesIDigress12!";

            if (await userManager.FindByEmailAsync(email) == null)
            {
                var user = new ApplicationUser();
                user.UserName = email.Split('@')[0];
                user.Email = email;
                user.FirstName = "Admin";
                user.LastName = "User";

                await userManager.CreateAsync(user, password);

                await userManager.AddToRoleAsync(user, "Admin");
            }
        }
        using (var scope = app.Services.CreateScope()) //Alison
        {
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            string email = "athornton@nmc.edu";
            string password = "Phish.Food.For.Thought88";

            if (await userManager.FindByEmailAsync(email) == null)
            {
                var user = new ApplicationUser();
                user.UserName = email.Split('@')[0];
                user.Email = email;
                user.FirstName = "Alison";
                user.LastName = "Thornton";

                await userManager.CreateAsync(user, password);

                await userManager.AddToRoleAsync(user, "Admin");
            }
        }

        app.Run();
    }

}

