using Eticaret.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace Eticaret.WebUI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddSession(options =>
            {
                options.Cookie.Name = ".Eticaret.Session";
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.IdleTimeout = TimeSpan.FromDays(1);
                options.IOTimeout = TimeSpan.FromMinutes(10);
            });

            builder.Services.AddDbContext<DatabaseContext>();

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
                m.AddPolicy("AdminPolicy",policy=>policy.RequireClaim(ClaimTypes.Role,"Admin"));
                m.AddPolicy("UserPolicy",policy=>policy.RequireClaim(ClaimTypes.Role,"Admin","User","Customer"));
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseSession(); //session kullanmaya yarar

            app.UseAuthentication(); // önce oturum açma 
            app.UseAuthorization();// sonra yetkilendirme
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
