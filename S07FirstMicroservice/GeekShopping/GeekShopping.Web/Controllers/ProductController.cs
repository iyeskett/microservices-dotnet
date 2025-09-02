using GeekShopping.Web.Models;
using GeekShopping.Web.Services.IServices;
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

        public async Task<IActionResult> Index()
        {
            var products = await _productService.FindAllProducts();
            return View(products);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Product product)
        {
            if (ModelState.IsValid)
            {
                var response = await _productService.CreateProduct(product);
                if (response != null) return RedirectToAction(nameof(Index));
            }

            return View(product);
        }

        public IActionResult Update(int id)
        {
            var product = _productService.FindProductById(id).Result;
            if (product.Id > 0) return View(product);
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Update(Product product)
        {
            if (ModelState.IsValid)
            {
                var response = await _productService.UpdateProduct(product);
                if (response != null) return RedirectToAction(nameof(Index));
            }

            return View(product);
        }

        public IActionResult Delete(int id)
        {
            var product = _productService.FindProductById(id).Result;
            if (product.Id > 0) return View(product);
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Product product)
        {
            var deleted = await _productService.DeleteProductById(product.Id);
            if (deleted) return RedirectToAction(nameof(Index));

            return View(product.Id);
        }
    }
}