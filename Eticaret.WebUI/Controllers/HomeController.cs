using System.Diagnostics;
using Eticaret.Core.Entities;
using Eticaret.Service.Abstract;
using Eticaret.WebUI.Models;
using Microsoft.AspNetCore.Mvc;

namespace Eticaret.WebUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly IService<Product> _serviceProduct;
        private readonly IService<Slider> _serviceSlider;
        private readonly IService<News> _serviceNews;
        private readonly IService<Contact> _serviceContact;

        public HomeController(IService<Product> service, IService<Slider> serviceSlider, IService<News> serviceNews, IService<Contact> serviceContact)
        {
            _serviceProduct = service;
            _serviceSlider = serviceSlider;
            _serviceNews = serviceNews;
            _serviceContact = serviceContact;
        }

        public async Task<IActionResult> Index()
        {
            var model = new HomePageViemModel()
            {
                Sliders = await _serviceSlider.GetAllAsync(),
                News = await _serviceNews.GetAllAsync(),
                Products = await _serviceProduct.GetAllAsync(p=>p.IsActive && p.IsHome)
            };
            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }
        //[Route("AccesDenied")]
        public IActionResult AccessDenied()
        {
            return View();
        }
        public IActionResult ContactUs()
        {
            return View();
        }
        [HttpPost]
        public  async Task<IActionResult> ContactUsAsync(Contact c)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _serviceContact.AddAsync(c);
                    var sonuc = await _serviceContact.SaveChangesAsync();
                    if (sonuc > 0)
                    {
                        TempData["Message"] = @"<div class=""alert alert-success alert-dismissible fade show"" role=""alert"">
                        <strong>Mesajýnýz Baþarýyla Gönderilmiþtir!</strong>
    <button type=""button"" class=""btn-close"" data-bs-dismiss=""alert"" aria-label=""Close""></button>
    </div>";
                        //await MailHelper.SendMailAsync(c);
                        return RedirectToAction("ContactUs");
                    }
                }
                catch
                {
                    ModelState.AddModelError("","Hata Oluþtu!");
                }
            }
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
