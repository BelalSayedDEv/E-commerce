using E_Commerce.Model;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Repository
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


        public async Task<List<Product>?> GetProductsAsync(int page, int pageSize, string? SearchTerm)
        {
            var query = Context.Products.AsQueryable();

            if (!string.IsNullOrWhiteSpace(SearchTerm))
                query = query.Where(p => p.Name.Contains(SearchTerm));

            Console.WriteLine(query.ToQueryString());
            return await query.Skip((page - 1) * pageSize).Take(pageSize).AsNoTracking().ToListAsync();
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

        public async Task<int?> GetProductCountAsync(string? searchTerm)
        {
            var query = Context.Products.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
                query = query.Where(p => p.Name.Contains(searchTerm));

            return await query.CountAsync();
        }
    }
}
