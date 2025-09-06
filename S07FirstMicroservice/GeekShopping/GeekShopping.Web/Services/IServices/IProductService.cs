using GeekShopping.Web.Models;

namespace GeekShopping.Web.Services.IServices
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> FindAllProducts(string token);

        Task<Product> FindProductById(long id, string token);

        Task<Product> CreateProduct(Product product, string token);

        Task<Product> UpdateProduct(Product product, string token);

        Task<bool> DeleteProductById(long id, string token);
    }
}