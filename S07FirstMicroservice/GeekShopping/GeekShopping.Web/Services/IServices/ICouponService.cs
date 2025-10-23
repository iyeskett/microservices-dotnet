using GeekShopping.Web.Models;

namespace GeekShopping.Web.Services.IServices
{
    public interface ICouponService
    {
        Task<Coupon> GetCoupon(string code, string token);
    }
}