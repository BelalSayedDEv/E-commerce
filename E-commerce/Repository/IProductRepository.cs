using E_Commerce.Model;

namespace E_Commerce.Repository
{
    public interface IProductRepository
    {
        public Task<List<Product>?> GetProductsAsync(int page, int pageSize, string? searchTerm);
        public Task<Product?> GetProductByIdAsync(int Id);
        public Task<int?> GetProductCountAsync(string? searchTerm);
        public Task<Product?> GetProductByNameAsync(string name);
        public Task AddProductAsync(Product product);
        public Task deleteProductAsync(int Id);

        public Task SaveAsync();
    }
}
