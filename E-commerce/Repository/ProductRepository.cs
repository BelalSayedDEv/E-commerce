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
        public List<Product>? GetProducts()
        {
            var products = Context.Products.ToList();
            return products;
        }

        public Product? GetProductById(int Id)
        {
            var product = Context.Products.First(d => d.Id == Id);

            return product;

        }


        public Product? GetProductByName(string name)
        {
            var product = Context.Products.FirstOrDefault(d => d.Name.ToLower() == name.ToLower());
            return product;
        }

        public void UpdateProduct(int Id, Product product)
        {
            var ProductFromdata = Context.Products.FirstOrDefault(d => d.Id == Id);
            if (ProductFromdata == null)
                return;

            ProductFromdata.Price = product.Price;
            ProductFromdata.Quantity = product.Quantity;
            ProductFromdata.Name = product.Name;
            ProductFromdata.Description = product.Description;

            Context.Products.Update(ProductFromdata);

        }

        public void deleteProduct(int Id)
        {
            var product = Context.Products.FirstOrDefault(d => d.Id == Id);

            if (product == null)
                return;
            Context.Products.Remove(product);

        }

        public void Save()
        {
            Context.SaveChanges();
        }

        public void AddProduct(Product product)
        {
            Context.Add(product);
        }





        // version with async

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
