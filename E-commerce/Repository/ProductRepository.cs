using E_commerce.Model;
using Microsoft.EntityFrameworkCore;

namespace E_commerce.Repository
{
    public class ProductRepository : IProductRepository
    {
        ApplicationDbContext Context;
        public ProductRepository(ApplicationDbContext context)
        {
            Context = context;
        }

        public async Task<Product?> GetProductByIdAsync(int Id)
        {
            var product = await Context.Products.FirstOrDefaultAsync(d => d.Id == Id);

            return product;

        }

        public async Task<List<Product>?> GetProductsAsync()
        {
            var products = await Context.Products.ToListAsync();

            return products.ToList();
        }

        public async Task<Product?> GetProductByNameAsync(string name)
        {
            var product = await Context.Products.FirstOrDefaultAsync(d => d.Name.ToLower() == name.ToLower());
            return product;
        }


        public async Task deleteProductAsync(int Id)
        {
            var product = await Context.Products.FirstOrDefaultAsync(d => d.Id == Id);
            if (product == null)
                return;

            Context.Products.Remove(product);

        }

        public async Task SaveAsync()
        {
            await Context.SaveChangesAsync();
        }

        public async Task AddProductAsync(Product product)
        {
            await Context.AddAsync(product);
        }


    }
}
