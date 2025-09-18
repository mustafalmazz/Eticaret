using Eticaret.Core.Entities;
using Eticaret.Data;
using Eticaret.Service.Abstract;
using Eticaret.WebUI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Eticaret.WebUI.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IService<Product> _service;

        public ProductsController(IService<Product> service)
        {
            _service = service;
        }
        public async Task<IActionResult> Index(string q = "")
        {
            var databasecontext = _service.GetAllAsync(p=>p.IsActive &&p.Name.Contains(q) || p.Description.Contains(q) || p.Brand.Name.Contains(q));
            return View(await databasecontext);
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            // Ürün bilgisi ve ilişkili tablolar
            var product = await _service.GetQueryable()
                                        .Include(p => p.Brand)
                                        .Include(p => p.Category)
                                        .Include(p => p.ProductImages)
                                        .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
                return NotFound();

            // Üst kategori Id'si
            int topCategoryId = product.Category.ParentId > 0 ? product.Category.ParentId : product.Category.Id;

            // İlişkili ürünler: aynı üst kategoriye bağlı tüm ürünler
            var relatedProducts = _service.GetQueryable()
                                          .Include(p => p.Category)
                                          .Where(p => p.IsActive &&
                                                      (p.Category.Id == topCategoryId || p.Category.ParentId == topCategoryId) &&
                                                      p.Id != product.Id)
                                          .Take(8); // Opsiyonel: max 8 ürün

            var model = new ProductDetaİlViewModel
            {
                Product = product,
                RelatedProducts = relatedProducts
            };

            return View(model);
        }
    }
}
