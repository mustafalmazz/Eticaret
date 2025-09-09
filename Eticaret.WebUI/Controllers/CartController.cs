using Eticaret.Core.Entities;
using Eticaret.Service.Abstract;
using Eticaret.Service.Concrete;
using Eticaret.WebUI.ExtensionMethods;
using Eticaret.WebUI.Models;
using Iyzipay;
using Iyzipay.Model;
using Iyzipay.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eticaret.WebUI.Controllers
{
    public class CartController : Controller
    {
        private readonly IService<Product> _serviceProduct;
        private readonly IService<Core.Entities.Address> _serviceAddress;
        private readonly IService<AppUser> _serviceAppUser;
        private readonly IService<Order> _serviceOrder;
        private readonly IConfiguration _configuration;

        public CartController(
            IService<Product> service,
            IService<Core.Entities.Address> serviceAddress,
            IService<AppUser> serviceAppUser,
            IService<Order> serviceOrder,
            IConfiguration configuration)
        {
            _serviceProduct = service;
            _serviceAddress = serviceAddress;
            _serviceAppUser = serviceAppUser;
            _serviceOrder = serviceOrder;
            _configuration = configuration;
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

        public IActionResult Add(int id, int quantity = 1)
        {
            var product = _serviceProduct.Find(id);
            if (product != null)
            {
                var cart = GetCart();
                cart.AddProduct(product, quantity);
                HttpContext.Session.SetJson("Cart", cart);
                return Redirect(Request.Headers["Referer"].ToString());
            }
            return RedirectToAction("Index");
        }

        public IActionResult Update(int id, int quantity = 1)
        {
            var product = _serviceProduct.Find(id);
            if (product != null)
            {
                var cart = GetCart();
                cart.UpdateProduct(product, quantity);
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
        public async Task<IActionResult> Checkout()
        {
            var cart = GetCart();
            var appUser = await _serviceAppUser.GetAsync(x => x.UserGuid.ToString() == HttpContext.User.FindFirst("UserGuid").Value);
            if (appUser == null)
            {
                return RedirectToAction("SignIn", "Account");
            }
            var addresses = await _serviceAddress.GetAllAsync(a => a.AppUserId == appUser.Id && a.IsActive);
            var model = new CheckOutViewModel()
            {
                CartProducts = cart.CartLines,
                TotalPrice = cart.TotalPrice(),
                Addresses = addresses
            };
            return View(model);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Checkout(string CardNumber, string CardNameSurname, string CardMonth, string CardYear, string CVV, string DeliveryAddress, string BillingAddress)
        {
            var cart = GetCart();
            var appUser = await _serviceAppUser.GetAsync(x => x.UserGuid.ToString() == HttpContext.User.FindFirst("UserGuid").Value);
            if (appUser == null)
            {
                return RedirectToAction("SignIn", "Account");
            }

            var addresses = await _serviceAddress.GetAllAsync(a => a.AppUserId == appUser.Id && a.IsActive);
            var model = new CheckOutViewModel()
            {
                CartProducts = cart.CartLines,
                TotalPrice = cart.TotalPrice(),
                Addresses = addresses
            };

            if (string.IsNullOrWhiteSpace(CardNumber) || string.IsNullOrWhiteSpace(CardYear) ||
                string.IsNullOrWhiteSpace(CardMonth) || string.IsNullOrWhiteSpace(CVV) ||
                string.IsNullOrWhiteSpace(DeliveryAddress) || string.IsNullOrWhiteSpace(BillingAddress))
            {
                return View(model);
            }

            var faturaAdresi = addresses.FirstOrDefault(x => x.AddressGuid.ToString() == BillingAddress);
            var teslimatAdresi = addresses.FirstOrDefault(x => x.AddressGuid.ToString() == DeliveryAddress);

            if (faturaAdresi == null || teslimatAdresi == null)
            {
                TempData["Message"] = "<div class='alert alert-danger'>Adres bulunamadı!</div>";
                return View(model);
            }

            var siparis = new Order
            {
                AppUserId = appUser.Id,
                BillingAddress = $"{faturaAdresi.OpenAddress} {faturaAdresi.District} {faturaAdresi.City}",
                CustomerId = appUser.UserGuid.ToString(),
                DeliveryAddress = $"{teslimatAdresi.OpenAddress} {teslimatAdresi.District} {teslimatAdresi.City}",
                OrderDate = DateTime.Now,
                TotalPrice = cart.TotalPrice(),
                OrderNumber = Guid.NewGuid().ToString(),
                OrderState = 0,
                OrderLines = []
            };

            #region OdemeIslemi
            Options options = new Options
            {
                ApiKey = _configuration["IyzicOptions:ApiKey"],
                SecretKey = _configuration["IyzicOptions:SecretKey"],
                BaseUrl = _configuration["IyzicOptions:BaseUrl"]
            };

            CreatePaymentRequest request = new CreatePaymentRequest
            {
                Locale = Locale.TR.ToString(),
                ConversationId = HttpContext.Session.Id,
                Price = siparis.TotalPrice.ToString().Replace(",", "."),
                PaidPrice = siparis.TotalPrice.ToString().Replace(",", "."),
                Currency = Currency.TRY.ToString(),
                Installment = 1,
                BasketId = "B" + HttpContext.Session.Id,
                PaymentChannel = PaymentChannel.WEB.ToString(),
                PaymentGroup = PaymentGroup.PRODUCT.ToString()
            };

            PaymentCard paymentCard = new PaymentCard
            {
                CardHolderName = CardNameSurname,
                CardNumber = CardNumber,
                ExpireMonth = CardMonth,
                ExpireYear = CardYear,
                Cvc = CVV,
                RegisterCard = 0
            };
            request.PaymentCard = paymentCard;

            Buyer buyer = new Buyer
            {
                Id = "BY" + appUser.Id,
                Name = appUser.Name,
                Surname = appUser.SurName,
                GsmNumber = appUser.Phone,
                Email = appUser.Email,
                IdentityNumber = "11111111111",
                LastLoginDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                RegistrationDate = appUser.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                RegistrationAddress = siparis.DeliveryAddress,
                Ip = HttpContext.Connection.RemoteIpAddress?.ToString(),
                City = teslimatAdresi.City,
                Country = "Turkey",
                ZipCode = "34732"
            };
            request.Buyer = buyer;

            var shippingAddress = new Iyzipay.Model.Address
            {
                ContactName = appUser.Name + " " + appUser.SurName,
                City = teslimatAdresi.City,
                Country = "Turkey",
                Description = teslimatAdresi.OpenAddress,
                ZipCode = "34742"
            };
            request.ShippingAddress = shippingAddress;

            var billingAddress = new Iyzipay.Model.Address
            {
                ContactName = appUser.Name + " " + appUser.SurName,
                City = faturaAdresi.City,
                Country = "Turkey",
                Description = faturaAdresi.OpenAddress,
                ZipCode = "34742"
            };
            request.BillingAddress = billingAddress;

            List<BasketItem> basketItems = new List<BasketItem>();
            foreach (var item in cart.CartLines)
            {
                siparis.OrderLines.Add(new OrderLine
                {
                    ProductId = item.Product.Id,
                    OrderId = siparis.Id,
                    Quantity = item.Quantity,
                    UnitPrice = item.Product.Price,
                });

                basketItems.Add(new BasketItem
                {
                    Id = item.Product.Id.ToString(),
                    Name = item.Product.Name,
                    Category1 = "Collectibles",
                    ItemType = BasketItemType.PHYSICAL.ToString(),
                    Price = (item.Product.Price * item.Quantity).ToString().Replace(",", ".")
                });
            }

            if (siparis.TotalPrice < 999)
            {
                basketItems.Add(new BasketItem
                {
                    Id = "Kargo",
                    Name = "Kargo Ücreti",
                    Category1 = "Kargo Ücreti",
                    ItemType = BasketItemType.VIRTUAL.ToString(),
                    Price = "99"
                });
                siparis.TotalPrice += 99;
                request.Price = siparis.TotalPrice.ToString().Replace(",", ".");
                request.PaidPrice = siparis.TotalPrice.ToString().Replace(",", ".");
            }

            request.BasketItems = basketItems;

            Payment payment = await Payment.Create(request, options);
            #endregion

            try
            {
                if (payment.Status == "success")
                {
                    await _serviceOrder.AddAsync(siparis);
                    var sonuc = await _serviceOrder.SaveChangesAsync();
                    if (sonuc > 0)
                    {
                        HttpContext.Session.Remove("Cart");
                        return RedirectToAction("Thanks");
                    }
                }
                else
                {
                    TempData["Message"] =
                        $"<div class='alert alert-danger'> Ödeme İşlemi Başarısız! </div>({payment.ErrorMessage ?? "Bilinmeyen hata."})";
                    return RedirectToAction("Checkout");
                }
            }
            catch (Exception ex)
            {
                TempData["Message"] = $"<div class='alert alert-danger'> Hata Oluştu! ({ex.Message}) </div>";
            }

            return View(model);
        }

        public IActionResult Thanks()
        {
            return View();
        }
    }
}
