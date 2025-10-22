using GeekShopping.CouponAPI.Model;
using GeekShopping.CouponAPI.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GeekShopping.CouponAPI.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CouponController : ControllerBase
    {

        private CouponRepository _repository;
        private readonly ILogger<CouponController> _logger;

        public CouponController(CouponRepository repository, ILogger<CouponController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        [HttpGet("{couponCode}")]
        [Authorize]
        public async Task<ActionResult<CouponDTO>> GetCouponByCouponCode(string couponCode)
        {
            try
            {
                var coupon = await _repository.GetCouponByCouponCode(couponCode);
                return Ok(coupon);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving coupon with code {couponCode}: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

    }
}
