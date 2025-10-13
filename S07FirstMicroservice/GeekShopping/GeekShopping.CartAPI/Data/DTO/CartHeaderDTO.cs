namespace GeekShopping.CartAPI.Data.DTO
{
    public class CartHeaderDTO
    {
        public string UserId { get; set; } = string.Empty;

        public string CouponCode { get; set; } = string.Empty;

        public decimal DiscountAmount { get; set; }

        public List<CartDetailDTO>? CartDetails { get; set; }
    }
}