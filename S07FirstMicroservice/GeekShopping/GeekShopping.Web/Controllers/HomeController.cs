using GeekShopping.Web.Models;
using GeekShopping.Web.Services.IServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace GeekShopping.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private IProductService _productService;
        private ICartService _cartService;
        private ICouponService _couponService;

        public HomeController(ILogger<HomeController> logger, IProductService productService, ICartService cartService, ICouponService couponService)
        {
            _logger = logger;
            _productService = productService;
            _cartService = cartService;
            _couponService = couponService;
        }

        public async Task<IActionResult> IndexAsync()
        {
            var products = await _productService.FindAllProducts(string.Empty);
            return View(products);
        }

        [Authorize]
        public async Task<IActionResult> Details(int id)
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var product = await _productService.FindProductById(id, accessToken);
            return View(product);
        }

        [HttpPost]
        [ActionName("Details")]
        [Authorize]
        public async Task<IActionResult> DetailsPost(Product product)
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");

            Cart cart = new()
            {
                CartHeader = new()
                {
                    UserId = User.Claims.Where(_ => _.Type == "sub")?.FirstOrDefault()?.Value
                }
            };

            CartDetail cartDetail = new()
            {
                Count = product.Count,
                ProductId = product.Id,
                Product = await _productService.FindProductById(product.Id, accessToken)
            };

            List<CartDetail> cartDetailsList = new()
            {
                cartDetail
            };

            cart.CartDetails = cartDetailsList;

            var response = await _cartService.AddItemToCart(cart, accessToken);
            if (response != null)
            {
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        [Authorize]
        public async Task<IActionResult> LoginAsync()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Logout()
        {
            return SignOut("Cookies", "oidc");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}