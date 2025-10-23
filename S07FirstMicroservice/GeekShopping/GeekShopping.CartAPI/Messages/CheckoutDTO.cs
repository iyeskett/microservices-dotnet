using GeekShopping.CartAPI.Data.DTO;
using GeekShopping.MessageBus;

namespace GeekShopping.CartAPI.Messages
{
    public class CheckoutDTO : BaseMessage
    {
        public string UserId { get; set; } = string.Empty;
        public string CouponCode { get; set; } = string.Empty;
        public decimal PurchaseAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime DateTime { get; set; }
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string CardNumber { get; set; } = string.Empty;
        public string CVV { get; set; } = string.Empty;
        public string ExpiryMothYear { get; set; } = string.Empty;
        public int CartTotalItems { get; set; }
        public IEnumerable<CartDetailDTO> CartDetailDTOs { get; set; } = new List<CartDetailDTO>();
    }
}