using GeekShopping.Web.Models;
using GeekShopping.Web.Services.IServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GeekShopping.Web.Controllers
{
    public class CartController : Controller
    {
        private readonly ILogger<CartController> _logger;
        private IProductService _productService;
        private ICartService _cartService;

        public CartController(ILogger<CartController> logger, IProductService productService, ICartService cartService)
        {
            _logger = logger;
            _productService = productService;
            _cartService = cartService;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {

            return View(await FindUserCart());
        }

        public async Task<IActionResult> Remove(int id )
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var userId = User.Claims.Where(_ => _.Type == "sub")?.FirstOrDefault()?.Value;

            var removed = await _cartService.RemoveFromCart(id, accessToken);

            if (removed)
            {
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        private async Task<Cart> FindUserCart()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var userId = User.Claims.Where(_ => _.Type == "sub")?.FirstOrDefault()?.Value;

            var response = await _cartService.FindCartByUserId(userId, accessToken);

            if (response?.CartHeader != null)
            {
                foreach (var detail in response.CartDetails)
                {

                    response.CartHeader.PurchaseAmount += detail.Product.Price * detail.Count;
                }
            }

            return response;
        }
    }
}
