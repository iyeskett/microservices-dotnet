using GeekShopping.Web.Models;
using GeekShopping.Web.Services.IServices;
using GeekShopping.Web.Utils;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GeekShopping.Web.Controllers
{
    public class ProductController : Controller
    {
        private IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var products = await _productService.FindAllProducts(accessToken);
            return View(products);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(Product product)
        {
            if (ModelState.IsValid)
            {
                var accessToken = await HttpContext.GetTokenAsync("access_token");
                var response = await _productService.CreateProduct(product, accessToken);
                if (response != null) return RedirectToAction(nameof(Index));
            }

            return View(product);
        }

        public async Task<IActionResult> UpdateAsync(int id)
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var product = _productService.FindProductById(id, accessToken).Result;
            if (product.Id > 0) return View(product);
            return NotFound();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Update(Product product)
        {
            if (ModelState.IsValid)
            {
                var accessToken = await HttpContext.GetTokenAsync("access_token");
                var response = await _productService.UpdateProduct(product, accessToken);
                if (response != null) return RedirectToAction(nameof(Index));
            }

            return View(product);
        }

        [Authorize]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var product = _productService.FindProductById(id, accessToken).Result;
            if (product.Id > 0) return View(product);
            return NotFound();
        }

        [HttpPost]
        [Authorize(Roles = Role.Admin)]
        public async Task<IActionResult> Delete(Product product)
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var deleted = await _productService.DeleteProductById(product.Id, accessToken);
            if (deleted) return RedirectToAction(nameof(Index));

            return View(product.Id);
        }
    }
}