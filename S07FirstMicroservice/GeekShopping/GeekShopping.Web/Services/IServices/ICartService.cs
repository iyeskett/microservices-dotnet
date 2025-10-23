using GeekShopping.Web.Models;

namespace GeekShopping.Web.Services.IServices
{
    public interface ICartService
    {
        Task<Cart> FindCartByUserId(string userId, string token);

        Task<Cart> AddItemToCart(Cart cart, string token);

        Task<Cart> UpdateCart(Cart cart, string token);

        Task<bool> RemoveFromCart(long cartId, string token);

        Task<bool> ApplyCoupon(Cart cart, string token);

        Task<bool> RemoveCoupon(string userId, string token);

        Task<bool> ClearCart(string userId, string token);

        Task<CartHeader> Checkout(CartHeader cartHeader, string token);
    }
}