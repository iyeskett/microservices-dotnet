using GeekShopping.ProductAPI.Data.DTO;
using GeekShopping.ProductAPI.Repository;
using Microsoft.AspNetCore.Mvc;

namespace GeekShopping.ProductAPI.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private IProductRepository _productRepository;

        public ProductController(IProductRepository productRepository)
        {
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> FindAll()
        {
            return Ok(await _productRepository.FindAll());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDTO>> FindById(long id)
        {
            var product = await _productRepository.FindById(id);
            if (product == null) return NotFound();
            return Ok(product);
        }

        [HttpPost]
        public async Task<ActionResult<ProductDTO>> Create([FromBody] ProductDTO productDTO)
        {
            if (productDTO == null) return BadRequest();
            var product = await _productRepository.Create(productDTO);
            return CreatedAtAction(nameof(FindById), new { id = product.Id }, product);
        }

        [HttpPut]
        public async Task<ActionResult<ProductDTO>> Update([FromBody] ProductDTO productDTO)
        {
            if (productDTO == null) return BadRequest();
            var product = await _productRepository.Update(productDTO);
            return Ok(product);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(long id)
        {
            var deleted = await _productRepository.Delete(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}