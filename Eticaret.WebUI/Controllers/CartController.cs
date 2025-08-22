using Eticaret.Core.Entities;
using Eticaret.Service.Abstract;
using Eticaret.Service.Concrete;
using Eticaret.WebUI.ExtensionMethods;
using Eticaret.WebUI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eticaret.WebUI.Controllers
{
    public class CartController : Controller
    {
        private readonly IService<Product> _serviceProduct;
        private readonly IService<Address> _serviceAddress;
        private readonly IService<AppUser> _serviceAppUser;

        public CartController(IService<Product> service, IService<Address> serviceAddress, IService<AppUser> serviceAppUser)
        {
            _serviceProduct = service;
            _serviceAddress = serviceAddress;
            _serviceAppUser = serviceAppUser;
        }
        public IActionResult Index()
        {
            var cart = GetCart();
            var model = new CartViewModel()
            {
                CartLines = cart.CartLines,
                TotalPrice = cart.TotalPrice()
            };
            return View(model);
        }
        private CartService GetCart()
        {
            return HttpContext.Session.GetJson<CartService>("Cart") ?? new CartService();
        }
        public IActionResult Add(int id,int quantity = 1)
        {
            var product = _serviceProduct.Find(id);
            if (product != null)
            {
                var cart = GetCart();
                cart.AddProduct(product , quantity);
                HttpContext.Session.SetJson("Cart", cart);
                return Redirect(Request.Headers["Referer"].ToString());
            }
            return RedirectToAction("Index");
        }
        public IActionResult Update(int id,int quantity = 1)
        {
            var product = _serviceProduct.Find(id);
            if (product != null)
            {
                var cart = GetCart();
                cart.UpdateProduct(product , quantity);
                HttpContext.Session.SetJson("Cart", cart);
            }
            return RedirectToAction("Index");
        }
        public IActionResult Remove(int ProductId)
        {
            var product = _serviceProduct.Find(ProductId);
            if (product != null)
            {
                var cart = GetCart();
                cart.RemoveProduct(product);
                HttpContext.Session.SetJson("Cart", cart);
            }
            return RedirectToAction("Index");
        }
        [Authorize]
        public  async Task<IActionResult> Checkout()
        {
            var cart = GetCart();
            var appUser= await _serviceAppUser.GetAsync(x=>x.UserGuid.ToString() == HttpContext.User.FindFirst("UserGuid").Value);
            if (appUser == null)
            {
                return RedirectToAction("SignIn","Account");
            }
            var addresses = await _serviceAddress.GetAllAsync(a=>a.AppUserId == appUser.Id && a.IsActive);
            var model = new CheckOutViewModel()
            {
                CartProducts = cart.CartLines,
                TotalPrice = cart.TotalPrice(),
                Addresses = addresses  
            };
            return View(model);
        }
    }
}
