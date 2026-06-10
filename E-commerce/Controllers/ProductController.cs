using E_commerce.DTos.ProductDTOs;
using E_commerce.Model;
using E_commerce.Services;
using Microsoft.AspNetCore.Mvc;

namespace E_commerce.Controllers
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
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await productService.GetAllProductsAsync();

            if (products == null)
                return NotFound(ApiResponse<object>.Failure("No Found"));

            return Ok(ApiResponse<List<ShowProductDto>>.Success(products));
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
