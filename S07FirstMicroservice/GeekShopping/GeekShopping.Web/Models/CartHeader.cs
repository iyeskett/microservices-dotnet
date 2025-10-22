namespace GeekShopping.Web.Models
{
    public class CartHeader
    {
        public long Id { get; set; }
        public string UserId { get; set; } = string.Empty;

        public string CouponCode { get; set; } = string.Empty;

        public decimal PurchaseAmount { get; set; }

        public List<CartDetail>? CartDetails { get; set; }
    }
}