using Eticaret.Core.Entities;
using Eticaret.Service.Abstract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Eticaret.WebUI.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly IService<Category> _service;

        public CategoriesController(IService<Category> service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Seçilen kategori
            var category = await _service.GetQueryable()
                                         .Include(c => c.Products)
                                         .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                return NotFound();
            }

            // Alt kategorilerin hepsini bul
            var allCategoryIds = await GetAllSubCategoryIdsAsync(id.Value);
            allCategoryIds.Add(id.Value); // üst kategoriyi de ekle

            // Tüm ürünleri topla
            var products = await _service.GetQueryable()
                                         .Where(c => allCategoryIds.Contains(c.Id))
                                         .SelectMany(c => c.Products)
                                         .ToListAsync();

            ViewBag.Category = category; // kategori bilgisini gönderdim
            return View(products);       // ürün listesi döndürülüyor
        }

        public async Task<IActionResult> List()
        {
            var categories = await _service.GetQueryable()
                                           .Where(c => c.IsActive)
                                           .OrderBy(c => c.OrderNo)
                                           .ToListAsync();
            return View(categories);
        }

        // 🔽 Alt kategori id’lerini recursive bulan yardımcı fonksiyon
        private async Task<List<int>> GetAllSubCategoryIdsAsync(int categoryId)
        {
            var subcategories = await _service.GetQueryable()
                                              .Where(c => c.ParentId == categoryId && c.IsActive)
                                              .ToListAsync();

            var ids = new List<int>();
            foreach (var sub in subcategories)
            {
                ids.Add(sub.Id);
                ids.AddRange(await GetAllSubCategoryIdsAsync(sub.Id));
            }

            return ids;
        }
    }
}
