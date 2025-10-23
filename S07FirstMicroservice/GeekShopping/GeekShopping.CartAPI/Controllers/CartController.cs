using GeekShopping.CartAPI.Data.DTO;
using GeekShopping.CartAPI.Messages;
using GeekShopping.CartAPI.RabbitMQSender;
using GeekShopping.CartAPI.Repository;
using Microsoft.AspNetCore.Mvc;

namespace GeekShopping.CartAPI.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CartController : ControllerBase
    {
        private ICartRepository _repository;
        private IRabbitMQMessageSender _rabbitMQMessageSender;

        public CartController(ICartRepository repository, IRabbitMQMessageSender rabbitMQMessageSender)
        {
            _repository = repository;
            _rabbitMQMessageSender = rabbitMQMessageSender;
        }

        [HttpGet("find-cart/{id}")]
        public async Task<ActionResult<CartDTO>> FindById(string id)
        {
            var cart = await _repository.FindCartByUserId(id);

            if (cart == null) return NotFound();
            return Ok(cart);
        }

        [HttpPost("add-cart")]
        public async Task<ActionResult<CartDTO>> AddCart(CartDTO cartDTO)
        {
            var cart = await _repository.SaveOrUpdateCart(cartDTO);

            if (cart == null) return NotFound();
            return Ok(cart);
        }

        [HttpPut("update-cart")]
        public async Task<ActionResult<CartDTO>> UpdateCart(CartDTO cartDTO)
        {
            var cart = await _repository.SaveOrUpdateCart(cartDTO);

            if (cart == null) return NotFound();
            return Ok(cart);
        }

        [HttpDelete("remove-cart/{id}")]
        public async Task<ActionResult<CartDTO>> RemoveCart(int id)
        {
            var status = await _repository.RemoveFromCart(id);

            if (!status) return BadRequest();
            return Ok(status);
        }

        [HttpPost("apply-coupon")]
        public async Task<ActionResult<CartDTO>> ApplyCoupon(CartDTO cartDTO)
        {
            var added = await _repository.ApplyCoupon(cartDTO.CartHeader.UserId, cartDTO.CartHeader.CouponCode);

            if (!added) return NotFound();
            return Ok(added);
        }

        [HttpDelete("remove-coupon/{userId}")]
        public async Task<ActionResult<CartDTO>> RemoveCoupon(string userid)
        {
            var removed = await _repository.RemoveCoupon(userid);

            if (!removed) return NotFound();
            return Ok(removed);
        }

        [HttpPost("checkout")]
        public async Task<ActionResult<CheckoutDTO>> Checkout(CheckoutDTO checkoutDTO)
        {
            if (checkoutDTO?.UserId == null) return BadRequest();

            var cart = await _repository.FindCartByUserId(checkoutDTO.UserId);

            if (cart == null) return NotFound();

            checkoutDTO.CartDetailDTOs = cart.CartDetails;
            checkoutDTO.DateTime = DateTime.Now;

            _rabbitMQMessageSender.SendMessageAsync(checkoutDTO, "checkoutqueue");

            return Ok(checkoutDTO);
        }
    }
}