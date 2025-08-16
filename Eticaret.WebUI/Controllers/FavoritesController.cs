﻿using Eticaret.Core.Entities;
using Eticaret.Data;
using Eticaret.WebUI.ExtensionMethods;
using Microsoft.AspNetCore.Mvc;

namespace Eticaret.WebUI.Controllers
{
    public class FavoritesController : Controller
    {
        private readonly DatabaseContext _context;
        public FavoritesController(DatabaseContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var favoriler = GetFavorites();
            return View(favoriler);
        }
        private List<Product> GetFavorites()
        {
            return HttpContext.Session.GetJson<List<Product>>("GetFavorites") ?? [];
        }
        public IActionResult Add(int ProductId)
        {
            var favoriler = GetFavorites();
            var product = _context.Products.Find(ProductId);
            if (product != null && !favoriler.Any(p=>p.Id == ProductId))
            {
                favoriler.Add(product);
                HttpContext.Session.SetJson("GetFavorites",favoriler);

            }
            return RedirectToAction("Index");
        }
        public IActionResult Remove(int ProductId)
        {
            var favoriler = GetFavorites();
            var product = _context.Products.Find(ProductId);
            if (product != null && favoriler.Any(p=>p.Id == ProductId))
            {
                favoriler.RemoveAll(i=>i.Id == product.Id);
                HttpContext.Session.SetJson("GetFavorites",favoriler);

            }
            return RedirectToAction("Index");
        }
    }
}
