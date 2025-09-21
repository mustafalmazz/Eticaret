using Eticaret.Data;
using Eticaret.Service.Abstract;
using Eticaret.Service.Concrete;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using System.Globalization;
using System.Security.Claims;

namespace Eticaret.WebUI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

          
            var turkishCulture = new CultureInfo("tr-TR");
            CultureInfo.DefaultThreadCurrentCulture = turkishCulture;
            CultureInfo.DefaultThreadCurrentUICulture = turkishCulture;

        
            builder.Services.AddControllersWithViews();

            builder.Services.AddSession(options =>
            {
                options.Cookie.Name = ".Eticaret.Session";
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.IdleTimeout = TimeSpan.FromDays(1);
                options.IOTimeout = TimeSpan.FromMinutes(10);
            });

           
            builder.Services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[] { turkishCulture };
                options.DefaultRequestCulture = new RequestCulture(turkishCulture);
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
            });

            builder.Services.AddDbContext<DatabaseContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DbConnection"));
            });

            builder.Services.AddScoped(typeof(IService<>), typeof(Service<>));

            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(a =>
            {
                a.LoginPath = "/Account/SignIn";
                a.AccessDeniedPath = "/Home/AccessDenied";
                a.Cookie.Name = "Account";
                a.Cookie.MaxAge = TimeSpan.FromDays(7);
                a.Cookie.IsEssential = true;
            });

            builder.Services.AddAuthorization(m =>
            {
                m.AddPolicy("AdminPolicy", policy => policy.RequireClaim(ClaimTypes.Role, "Admin"));
                m.AddPolicy("UserPolicy", policy => policy.RequireClaim(ClaimTypes.Role, "Admin", "User", "Customer"));
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseRequestLocalization();

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseSession(); 
            app.UseAuthentication(); 
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "admin",
                pattern: "{area:exists}/{controller=Main}/{action=Index}/{id?}");

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}