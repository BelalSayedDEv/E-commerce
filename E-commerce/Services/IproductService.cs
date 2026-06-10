using E_commerce.DTos.ProductDTOs;

namespace E_commerce.Services
{
    public interface IproductService
    {
        public List<ShowProductDto> GetAllProducts();

        public ShowProductDto GetProductById(int productId);

        public ShowProductDto AddProduct(AddProductDTO Product);

        public EditProductDto EditProduct(int id, EditProductDto Product);

        public int? DeleteProduct(int productId);



        // version with async

        public Task<ShowProductDto?> GetProductByIdAsync(int productId);
        public Task<List<ShowProductDto>> GetAllProductsAsync();
        public Task<ShowProductDto?> AddProductAsync(AddProductDTO productFromReq);

        public Task<EditProductDto?> EditProductAsync(int id, EditProductDto ProductFromReq);

        public Task<int?> DeleteProductAsync(int productId);

        public Task<ShowProductDto?> EditeProductStockAsync(int Id, UpdateProductStock updateProductStock);

    }
}
