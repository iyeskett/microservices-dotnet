namespace GeekShopping.Web.Models
{
    public class Coupon
    {
        public long Id { get; set; }
        public string CouponCode { get; set; } = string.Empty;

        public decimal DiscountAmount { get; set; }
    }
}
