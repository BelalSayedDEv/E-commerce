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
        //private readonly IProductRepository productRepository;
        private readonly IproductService productService;

        public ProductController(IproductService iproductService)
        {
            //this.productRepository = productRepository;
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
            // Old Version 

            //var Product = productRepository.GetProductById(Id);
            //if (Product == null)
            //    return NotFound();

            //var DtoProduct = new AddProductDTO();

            //DtoProduct.Name = Product.Name;
            //DtoProduct.Description = Product.Description;
            //DtoProduct.Price = Product.Price;
            //DtoProduct.CategoryID = Product.CategoryID;

            //return Ok(DtoProduct);


            var Product = await productService.GetProductByIdAsync(Id);

            if (Product == null)
                return NotFound(ApiResponse<object>.Failure("No Found"));

            return Ok(ApiResponse<ShowProductDto>.Success(Product));
        }

        [HttpPost]
        public async Task<ActionResult> AddProduct(AddProductDTO ProductFromReq)
        {

            //Old Version

            //var Product = productRepository.GetProductByName(product.Name);
            //if (Product != null)
            //    return BadRequest("Name of Product is Exist");

            //var SProduct = new Product();
            //SProduct.Name = product.Name;
            //SProduct.Description = product.Description;
            //SProduct.Price = product.Price;
            //SProduct.CategoryID = product.CategoryID;


            //productRepository.AddProduct(SProduct);
            //productRepository.Save();

            //return CreatedAtAction("GetProductById", new { Id = SProduct.Id }, product);



            // ------------------------------- version 2 -----------------------------------

            var product = await productService.AddProductAsync(ProductFromReq);

            if (product == null)
                return BadRequest(ApiResponse<Object>.Failure("Product has already exist"));

            return CreatedAtAction("GetProductById", new { Id = product.Id }, ApiResponse<ShowProductDto>.Success(product));

        }

        [HttpPut("{Id}")]
        public async Task<IActionResult> EditProduct(int Id, EditProductDto ProductFromReq)
        {
            //--------------------------- version 1 ----------------------------
            //var ProductfromDatabase = productRepository.GetProductById(Id);
            //if (ProductfromDatabase == null)
            //    return NotFound();

            //productRepository.UpdateProduct(Id, product);
            //productRepository.Save();
            //return NoContent();


            //----------------------------- version 2 ----------------------------

            EditProductDto? product = await productService.EditProductAsync(Id, ProductFromReq);

            if (product == null)
                return NotFound(ApiResponse<object>.Failure("No Found"));

            return Ok(ApiResponse<EditProductDto>.Success(product));

        }

        [HttpDelete]
        public async Task<IActionResult> DeleteProduct(int Id)
        {
            //---------------------- version 1 ----------------------------------


            //    var Product = productRepository.GetProductById(Id);

            //    if (Product == null)
            //        return NotFound();

            //    productRepository.deleteProduct(Id);
            //    productRepository.Save();
            //    return NoContent();
            //}

            // ----------------------- version 2 ---------------------------------

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
