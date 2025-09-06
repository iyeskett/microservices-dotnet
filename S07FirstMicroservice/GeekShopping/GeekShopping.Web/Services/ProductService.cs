using GeekShopping.Web.Models;
using GeekShopping.Web.Services.IServices;
using System.Net.Http.Headers;

namespace GeekShopping.Web.Services
{
    public class ProductService : IProductService
    {
        private readonly HttpClient _httpClient;
        public const string BasePath = "api/v1/product";

        public ProductService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<Product>> FindAllProducts(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.GetAsync(BasePath);
            return await response.Content.ReadFromJsonAsync<List<Product>>() ?? new();
        }

        public async Task<Product> FindProductById(long id, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.GetAsync($"{BasePath}/{id}");
            return await response.Content.ReadFromJsonAsync<Product>() ?? new();
        }

        public async Task<Product> CreateProduct(Product product, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.PostAsJsonAsync(BasePath, product);
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<Product>() ?? new();
            else
                throw new Exception("Something went wrong when calling API");
        }

        public async Task<Product> UpdateProduct(Product product, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.PutAsJsonAsync(BasePath, product);
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<Product>() ?? new();
            else
                throw new Exception("Something went wrong when calling API");
        }

        public async Task<bool> DeleteProductById(long id, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.DeleteAsync($"{BasePath}/{id}");
            if (response.IsSuccessStatusCode)
                return true;
            else
                throw new Exception("Something went wrong when calling API");
        }
    }
}