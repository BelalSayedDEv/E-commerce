using E_commerce.Model;

namespace E_commerce.Repository
{
    public interface IProductRepository
    {
        public List<Product>? GetProducts();

        public Product? GetProductById(int id);

        public Product? GetProductByName(string name);

        public void AddProduct(Product product);
        public void UpdateProduct(int Id, Product product);

        public void deleteProduct(int Id);

        public void Save();


        /// <summary>
        /// this is diference between async and sync
        /// </summary>
        /// 
        public Task<List<Product>?> GetProductsAsync();
        public Task<Product?> GetProductByIdAsync(int Id);

        public Task<Product?> GetProductByNameAsync(string name);

        public Task AddProductAsync(Product product);

        public Task deleteProductAsync(int Id);

        public Task SaveAsync();
    }
}
