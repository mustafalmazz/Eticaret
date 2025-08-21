using Eticaret.Core.Entities;

namespace Eticaret.WebUI.Models
{
    public class CheckOutViewModel
    {
        public List<CartLine>?  CartProducts { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
