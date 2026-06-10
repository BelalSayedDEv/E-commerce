using E_commerce.Model;

namespace E_commerce.Repository
{
    public interface IProductRepository
    {
        public Task<List<Product>?> GetProductsAsync();
        public Task<Product?> GetProductByIdAsync(int Id);

        public Task<Product?> GetProductByNameAsync(string name);

        public Task AddProductAsync(Product product);

        public Task deleteProductAsync(int Id);

        public Task SaveAsync();
    }
}
