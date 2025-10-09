using GeekShopping.CartAPI.Model.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace GeekShopping.CartAPI.Model
{
    [Table("cart_header")]
    public class CartHeader : BaseEntity
    {
        [Column("user_id")]
        public string UserId { get; set; } = string.Empty;

        [Column("coupon_code")]
        public string CouponCode { get; set; } = string.Empty;

        [NotMapped]
        public decimal DiscountAmount { get; set; }

        public List<CartDetail> CartDetails { get; set; } = new List<CartDetail>();
    }
}