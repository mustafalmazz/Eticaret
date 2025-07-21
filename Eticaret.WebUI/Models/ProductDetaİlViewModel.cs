using Eticaret.Core.Entities;

namespace Eticaret.WebUI.Models
{
    public class ProductDetaİlViewModel
    {
        public Product? Product { get; set; }
        public IEnumerable<Product>? RelatedProducts { get; set; }
    }
}
