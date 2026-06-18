using E_Commerce.DTos.ProductDTOs;
using E_Commerce.Model;
using E_Commerce.Services;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IproductService productService;

        public ProductController(IproductService iproductService)
        {
            this.productService = iproductService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts([FromQuery] int Page, [FromQuery] int PageSize, [FromQuery] string? searchTerm)
        {
            var products = await productService.GetAllProductsAsync(Page, PageSize, searchTerm);

            if (products == null || products.ProductList == null || products.TotalCount == 0)
                return NotFound(ApiResponse<object>.Failure("No Found"));

            return Ok(ApiResponse<ProductCountWithList>.Success(products));
        }


        [HttpGet("{Id}")]
        public async Task<ActionResult<AddProductDTO>> GetProductById(int Id)
        {

            var Product = await productService.GetProductByIdAsync(Id);

            if (Product == null)
                return NotFound(ApiResponse<object>.Failure("No Found"));

            return Ok(ApiResponse<ShowProductDto>.Success(Product));
        }

        [HttpPost]
        public async Task<ActionResult> AddProduct(AddProductDTO ProductFromReq)
        {

            var product = await productService.AddProductAsync(ProductFromReq);

            if (product == null)
                return BadRequest(ApiResponse<Object>.Failure("Product has already exist"));

            return CreatedAtAction("GetProductById", new { Id = product.Id }, ApiResponse<ShowProductDto>.Success(product));

        }

        [HttpPut("{Id}")]
        public async Task<IActionResult> EditProduct(int Id, EditProductDto ProductFromReq)
        {


            EditProductDto? product = await productService.EditProductAsync(Id, ProductFromReq);

            if (product == null)
                return NotFound(ApiResponse<object>.Failure("No Found"));

            return Ok(ApiResponse<EditProductDto>.Success(product));

        }

        [HttpDelete]
        public async Task<IActionResult> DeleteProduct(int Id)
        {

            var Product = await productService.DeleteProductAsync(Id);
            if (Product == null)
                return NotFound(ApiResponse<object>.Failure("No Found"));

            return Ok(ApiResponse<Object>.Success(null, "Deleted Successfully"));

        }

        [HttpPut("UpdateStock/Id")]
        public async Task<IActionResult> UpdateProductStock(int Id, UpdateProductStock stock)
        {
            var Prodcut = await productService.EditeProductStockAsync(Id, stock);
            if (Prodcut == null)
                return NotFound(ApiResponse<Object>.Failure("Not Found"));

            return Ok(ApiResponse<ShowProductDto>.Success(Prodcut));
        }

    }
}
