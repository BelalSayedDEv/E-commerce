using Assinments.DTos.ProductDTOs;
using Assinments.Model;
using Assinments.Repository;

namespace Assinments.Services
{
    public class ProductService : IproductService
    {
        private readonly IProductRepository productRepository;

        public ProductService(IProductRepository productRepository)
        {
            this.productRepository = productRepository;
        }




        public List<ShowProductDto> GetAllProducts()
        {
            var Products = productRepository.GetProducts().Select(p => new ShowProductDto
            {
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                CategoryID = p.CategoryID,
                Quantity = p.Quantity,
                Id = p.Id
            });
            return Products.ToList();
        }

        public ShowProductDto? GetProductById(int productId)
        {
            var Product = productRepository.GetProductById(productId);

            if (Product == null)
                return null;

            ShowProductDto ShowProductDto = new ShowProductDto();

            ShowProductDto.Name = Product.Name;
            ShowProductDto.Description = Product.Description;
            ShowProductDto.Price = Product.Price;
            ShowProductDto.Quantity = Product.Quantity;
            ShowProductDto.CategoryID = Product.CategoryID;

            return ShowProductDto;
        }


        public ShowProductDto? AddProduct(AddProductDTO productFromReq)
        {
            var ProductForDb = productRepository.GetProductByName(productFromReq.Name);

            if (ProductForDb != null)
                return null;

            Product product = new Product();


            product.Name = productFromReq.Name;
            product.Description = productFromReq.Description;
            product.Price = productFromReq.Price;
            product.Quantity = productFromReq.Quantity;
            product.CategoryID = productFromReq.CategoryID;

            productRepository.AddProduct(product);
            productRepository.Save();

            ShowProductDto showProduct = new ShowProductDto();

            showProduct.Id = product.Id;
            showProduct.Name = product.Name;
            showProduct.Description = product.Description;
            showProduct.Price = product.Price;
            showProduct.Quantity = product.Quantity;
            showProduct.CategoryID = product.Quantity;


            return showProduct;
        }


        public EditProductDto? EditProduct(int id, EditProductDto ProductFromReq)
        {
            var product = productRepository.GetProductById(id);

            if (product == null)
                return null;


            product.Name = ProductFromReq.Name;
            product.Description = ProductFromReq.Description;
            product.Price = ProductFromReq.Price;
            product.Quantity = ProductFromReq.Quantity;
            product.CategoryID = ProductFromReq.CategoryID;


            productRepository.Save();


            return (ProductFromReq);

        }

        public int? DeleteProduct(int productId)
        {
            var Product = productRepository.GetProductById(productId);

            if (Product == null)
                return null;

            productRepository.deleteProduct(productId);
            productRepository.Save();

            return 1;
        }


        // version with async
        public async Task<List<ShowProductDto>> GetAllProductsAsync()
        {
            var Products = await productRepository.GetProductsAsync();

            return Products.Select(p => new ShowProductDto
            {
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                CategoryID = p.CategoryID,
                Quantity = p.Quantity,
                Id = p.Id
            }).ToList();

        }
        public async Task<ShowProductDto?> GetProductByIdAsync(int productId)
        {

            var Product = await productRepository.GetProductByIdAsync(productId);

            if (Product == null)
                return null;

            ShowProductDto ShowProductDto = new ShowProductDto();

            ShowProductDto.Name = Product.Name;
            ShowProductDto.Description = Product.Description;
            ShowProductDto.Price = Product.Price;
            ShowProductDto.Quantity = Product.Quantity;
            ShowProductDto.CategoryID = Product.CategoryID;

            return ShowProductDto;
        }


        public async Task<ShowProductDto?> AddProductAsync(AddProductDTO productFromReq)
        {
            var ProductForDb = await productRepository.GetProductByNameAsync(productFromReq.Name);

            if (ProductForDb != null)
                return null;

            Product product = new Product();


            product.Name = productFromReq.Name;
            product.Description = productFromReq.Description;
            product.Price = productFromReq.Price;
            product.Quantity = productFromReq.Quantity;
            product.CategoryID = productFromReq.CategoryID;

            await productRepository.AddProductAsync(product);
            await productRepository.SaveAsync();

            ShowProductDto showProduct = new ShowProductDto();

            showProduct.Id = product.Id;
            showProduct.Name = product.Name;
            showProduct.Description = product.Description;
            showProduct.Price = product.Price;
            showProduct.Quantity = product.Quantity;
            showProduct.CategoryID = product.Quantity;


            return showProduct;
        }

        public async Task<EditProductDto?> EditProductAsync(int id, EditProductDto ProductFromReq)
        {
            var product = await productRepository.GetProductByIdAsync(id);

            if (product == null)
                return null;


            product.Name = ProductFromReq.Name;
            product.Description = ProductFromReq.Description;
            product.Price = ProductFromReq.Price;
            product.Quantity = ProductFromReq.Quantity;
            product.CategoryID = ProductFromReq.CategoryID;


            await productRepository.SaveAsync();

            return (ProductFromReq);

        }

        public async Task<int?> DeleteProductAsync(int productId)
        {
            var Product = await productRepository.GetProductByIdAsync(productId);

            if (Product == null)
                return null;

            await productRepository.deleteProductAsync(productId);
            await productRepository.SaveAsync();

            return 1;
        }


        public async Task<ShowProductDto?> EditeProductStockAsync(int Id, UpdateProductStock updateProductStock)
        {
            var product = productRepository.GetProductById(Id);


            if (product == null)
                return null;

            product.Quantity = updateProductStock.Quantity;


            await productRepository.SaveAsync();

            ShowProductDto ShowProductDto = new ShowProductDto();

            ShowProductDto.Id = product.Id;
            ShowProductDto.Name = product.Name;
            ShowProductDto.Description = product.Description;
            ShowProductDto.Price = product.Price;
            ShowProductDto.Quantity = product.Quantity;
            ShowProductDto.CategoryID = product.CategoryID;

            return ShowProductDto;
        }
    }
}
