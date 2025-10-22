using GeekShopping.CouponAPI.Model.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GeekShopping.CouponAPI.Model
{
    public class CouponDTO
    {
        public long Id { get; set; }
        public string CouponCode { get; set; } = string.Empty;

        public decimal DiscountAmount { get; set; }
    }
}