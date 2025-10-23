using GeekShopping.Web.Models;
using GeekShopping.Web.Services.IServices;
using System.Net;
using System.Net.Http.Headers;

namespace GeekShopping.Web.Services
{
    public class CartService : ICartService
    {
        private readonly HttpClient _httpClient;
        public const string BasePath = "api/v1/cart";

        public CartService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Cart> FindCartByUserId(string userId, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.GetAsync($"{BasePath}/find-cart/{userId}");
            return await response.Content.ReadFromJsonAsync<Cart>() ?? new();
        }

        public async Task<Cart> AddItemToCart(Cart cart, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.PostAsJsonAsync($"{BasePath}/add-cart", cart);
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<Cart>() ?? new();
            else
                throw new Exception("Something went wrong when calling API");
        }

        public async Task<Cart> UpdateCart(Cart cart, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.PutAsJsonAsync($"{BasePath}/update-cart", cart);
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<Cart>() ?? new();
            else
                throw new Exception("Something went wrong when calling API");
        }

        public async Task<bool> RemoveFromCart(long cartId, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.DeleteAsync($"{BasePath}/remove-cart/{cartId}");
            if (response.IsSuccessStatusCode)
                return true;
            else
                throw new Exception("Something went wrong when calling API");
        }

        public async Task<bool> ApplyCoupon(Cart cart, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.PostAsJsonAsync($"{BasePath}/apply-coupon/", cart);
            if (response.IsSuccessStatusCode)
                return true;
            else
                throw new Exception("Something went wrong when calling API");
        }

        public async Task<bool> RemoveCoupon(string userId, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.DeleteAsync($"{BasePath}/remove-coupon/{userId}");
            if (response.IsSuccessStatusCode)
                return true;
            else
                throw new Exception("Something went wrong when calling API");
        }

        public async Task<object> Checkout(CartHeader cartHeader, string token)
        {
            if (cartHeader.CouponCode == null) cartHeader.CouponCode = string.Empty;
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.PostAsJsonAsync($"{BasePath}/checkout/", cartHeader);
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<CartHeader>();
            else if (response.StatusCode == HttpStatusCode.PreconditionFailed)
                return "Coupon Price has changed, please confirm!";
            else
                throw new Exception("Something went wrong when calling API");
        }

        public async Task<bool> ClearCart(string userId, string token)
        {
            throw new NotImplementedException();
        }
    }
}