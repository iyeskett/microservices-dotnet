using GeekShopping.Web.Models;
using GeekShopping.Web.Services.IServices;
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

        public async Task<bool> ApplyCoupon(Cart cart, string couponCode, string token)
        {
            throw new NotImplementedException();
        }

        public async Task<Cart> Checkout(CartHeader cartHeader, string token)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> ClearCart(string userId, string token)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> RemoveCoupon(string userId, string token)
        {
            throw new NotImplementedException();
        }
    }
}