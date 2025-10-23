using GeekShopping.CartAPI.Data.DTO;
using GeekShopping.CartAPI.Messages;
using GeekShopping.CartAPI.RabbitMQSender;
using GeekShopping.CartAPI.Repository;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace GeekShopping.CartAPI.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CartController : ControllerBase
    {
        private ICartRepository _cartRepository;
        private IRabbitMQMessageSender _rabbitMQMessageSender;
        private ICouponRepository _couponRepository;

        public CartController(ICartRepository cartRepository, IRabbitMQMessageSender rabbitMQMessageSender, ICouponRepository couponRepository)
        {
            _cartRepository = cartRepository;
            _rabbitMQMessageSender = rabbitMQMessageSender;
            _couponRepository = couponRepository;
        }

        [HttpGet("find-cart/{id}")]
        public async Task<ActionResult<CartDTO>> FindById(string id)
        {
            var cart = await _cartRepository.FindCartByUserId(id);

            if (cart == null) return NotFound();
            return Ok(cart);
        }

        [HttpPost("add-cart")]
        public async Task<ActionResult<CartDTO>> AddCart(CartDTO cartDTO)
        {
            var cart = await _cartRepository.SaveOrUpdateCart(cartDTO);

            if (cart == null) return NotFound();
            return Ok(cart);
        }

        [HttpPut("update-cart")]
        public async Task<ActionResult<CartDTO>> UpdateCart(CartDTO cartDTO)
        {
            var cart = await _cartRepository.SaveOrUpdateCart(cartDTO);

            if (cart == null) return NotFound();
            return Ok(cart);
        }

        [HttpDelete("remove-cart/{id}")]
        public async Task<ActionResult<CartDTO>> RemoveCart(int id)
        {
            var status = await _cartRepository.RemoveFromCart(id);

            if (!status) return BadRequest();
            return Ok(status);
        }

        [HttpPost("apply-coupon")]
        public async Task<ActionResult<CartDTO>> ApplyCoupon(CartDTO cartDTO)
        {
            var added = await _cartRepository.ApplyCoupon(cartDTO.CartHeader.UserId, cartDTO.CartHeader.CouponCode);

            if (!added) return NotFound();
            return Ok(added);
        }

        [HttpDelete("remove-coupon/{userId}")]
        public async Task<ActionResult<CartDTO>> RemoveCoupon(string userid)
        {
            var removed = await _cartRepository.RemoveCoupon(userid);

            if (!removed) return NotFound();
            return Ok(removed);
        }

        [HttpPost("checkout")]
        public async Task<ActionResult<CheckoutDTO>> Checkout(CheckoutDTO checkoutDTO)
        {
            if (checkoutDTO?.UserId == null) return BadRequest();

            string token = await HttpContext.GetTokenAsync("access_token");

            var cart = await _cartRepository.FindCartByUserId(checkoutDTO.UserId);

            if (cart == null) return NotFound();
            if (!string.IsNullOrEmpty(checkoutDTO.CouponCode))
            {
                CouponDTO couponDTO = await _couponRepository.GetCouponByCouponCode(checkoutDTO.CouponCode, token);

                if (couponDTO.DiscountAmount != checkoutDTO.DiscountAmount)
                {
                    return StatusCode(412);
                }
            }

            checkoutDTO.CartDetailDTOs = cart.CartDetails;
            checkoutDTO.DateTime = DateTime.Now;

            _rabbitMQMessageSender.SendMessageAsync(checkoutDTO, "checkoutqueue");

            await _cartRepository.ClearCart(checkoutDTO.UserId);

            return Ok(checkoutDTO);
        }
    }
}