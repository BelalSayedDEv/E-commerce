using E_commerce.DTos.ProductDTOs;

namespace E_commerce.Services
{
    public interface IproductService
    {

        public Task<ShowProductDto?> GetProductByIdAsync(int productId);
        public Task<List<ShowProductDto>> GetAllProductsAsync();
        public Task<ShowProductDto?> AddProductAsync(AddProductDTO productFromReq);

        public Task<EditProductDto?> EditProductAsync(int id, EditProductDto ProductFromReq);

        public Task<int?> DeleteProductAsync(int productId);

        public Task<ShowProductDto?> EditeProductStockAsync(int Id, UpdateProductStock updateProductStock);

    }
}
