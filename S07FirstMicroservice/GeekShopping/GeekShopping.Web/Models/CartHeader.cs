namespace GeekShopping.Web.Models
{
    public class CartHeader
    {
        public string UserId { get; set; } = string.Empty;

        public string CouponCode { get; set; } = string.Empty;

        public decimal DiscountAmount { get; set; }

        public List<CartDetail>? CartDetails { get; set; }
    }
}