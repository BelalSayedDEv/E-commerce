using E_Commerce.DTos.ProductDTOs;

namespace E_Commerce.Services
{
    public interface IproductService
    {

        public Task<ShowProductDto?> GetProductByIdAsync(int productId);
        public Task<ProductCountWithList> GetAllProductsAsync(int Page, int PageSize, string? searchTerm);
        public Task<ShowProductDto?> AddProductAsync(AddProductDTO productFromReq);

        public Task<EditProductDto?> EditProductAsync(int id, EditProductDto ProductFromReq);

        public Task<int?> DeleteProductAsync(int productId);

        public Task<ShowProductDto?> EditeProductStockAsync(int Id, UpdateProductStock updateProductStock);

    }
}
