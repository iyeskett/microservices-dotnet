using GeekShopping.Web.Models;
using GeekShopping.Web.Services.IServices;

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

        public async Task<IEnumerable<Product>> FindAllProducts()
        {
            var response = await _httpClient.GetAsync(BasePath);
            return await response.Content.ReadFromJsonAsync<List<Product>>() ?? [];
        }

        public async Task<Product> FindProductById(long id)
        {
            var response = await _httpClient.GetAsync($"{BasePath}/{id}");
            return await response.Content.ReadFromJsonAsync<Product>() ?? new();
        }

        public async Task<Product> CreateProduct(Product product)
        {
            var response = await _httpClient.PostAsJsonAsync(BasePath, product);
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<Product>() ?? new();
            else
                throw new Exception("Something went wrong when calling API");
        }

        public async Task<Product> UpdateProduct(Product product)
        {
            var response = await _httpClient.PutAsJsonAsync(BasePath, product);
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<Product>() ?? new();
            else
                throw new Exception("Something went wrong when calling API");
        }

        public async Task<bool> DeleteProductById(long id)
        {
            var response = await _httpClient.DeleteAsync($"{BasePath}/{id}");
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<bool>();
            else
                throw new Exception("Something went wrong when calling API");
        }
    }
}