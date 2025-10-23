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
        private ICouponService _couponService;

        public CartController(ILogger<CartController> logger, IProductService productService, ICartService cartService, ICouponService couponService)
        {
            _logger = logger;
            _productService = productService;
            _cartService = cartService;
            _couponService = couponService;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            return View(await FindUserCart());
        }

        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            return View(await FindUserCart());
        }

        [HttpPost]
        public async Task<IActionResult> Checkout(Cart cart)
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");

            var response = await _cartService.Checkout(cart.CartHeader, accessToken);

            if (response != null && response.GetType() == typeof(string))
            {
                TempData["Error"] = response;
                return RedirectToAction(nameof(Checkout));
            }

            if (response != null)
            {
                return RedirectToAction(nameof(Confirmation));
            }
            return View(cart);
        }

        public IActionResult Confirmation()
        {
            return View();
        }

        [HttpPost]
        [ActionName("ApplyCoupon")]
        public async Task<IActionResult> ApplyCoupon(Cart cart)
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var userId = User.Claims.Where(_ => _.Type == "sub")?.FirstOrDefault()?.Value;

            var saved = await _cartService.ApplyCoupon(cart, accessToken);

            if (saved)
            {
                return RedirectToAction(nameof(Index));
            }
            return View(await FindUserCart());
        }

        [HttpPost]
        [ActionName("RemoveCoupon")]
        public async Task<IActionResult> RemoveCoupon()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var userId = User.Claims.Where(_ => _.Type == "sub")?.FirstOrDefault()?.Value;

            var removed = await _cartService.RemoveCoupon(userId, accessToken);

            if (removed)
            {
                return RedirectToAction(nameof(Index));
            }
            return View(await FindUserCart());
        }

        public async Task<IActionResult> Remove(int id)
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
                if (!string.IsNullOrWhiteSpace(response.CartHeader.CouponCode))
                {
                    var coupon = await _couponService.GetCoupon(response.CartHeader.CouponCode, accessToken);
                    if (coupon.CouponCode != null)
                    {
                        response.CartHeader.DiscountAmount = coupon.DiscountAmount;
                    }
                }

                foreach (var detail in response.CartDetails)
                {
                    response.CartHeader.PurchaseAmount += detail.Product.Price * detail.Count;
                }

                response.CartHeader.PurchaseAmount -= response.CartHeader.DiscountAmount;
            }

            return response;
        }
    }
}