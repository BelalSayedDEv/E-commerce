using E_commerce.DTos;
using E_commerce.DTos.CategoryDTOs;
using E_commerce.Model;
using E_commerce.Services;
using Microsoft.AspNetCore.Mvc;

namespace E_commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        //private readonly ICategoryRepository categoryRepository;
        private readonly ICategoryService categoryService;

        public CategoryController(ICategoryService categoryService)
        {

            this.categoryService = categoryService;
        }

        [HttpGet]
        public ActionResult<List<ShowCategoryDto>> GetAllCategory()
        {
            //---------------------- version 1

            //var Listdto = new List<ShowCategoryDto>();
            //var Categories = categoryRepository.GetCategories();

            //if (Categories == null)
            //    return NotFound();

            //foreach (var item in Categories)
            //{
            //    var dto = new ShowCategoryDto();
            //    dto.Name = item.Name;
            //    dto.ProductsName = item.Products.Select(p => p.Name).ToList();
            //    Listdto.Add(dto);
            //}

            //return Ok(Listdto);
            // -----------------------------version 2

            List<ShowCategoryDto> categories = categoryService.GetCategories().ToList();
            if (categories == null)
                return NotFound(ApiResponse<object>.Failure("Not Found"));


            return Ok(ApiResponse<List<ShowCategoryDto>>.Success(categories));
        }

        [HttpGet("{Id}")]
        public ActionResult<ShowCategoryDto> GetCategorybyId(int Id)
        {

            //--------------------- version 1 
            //var ExistingCategory = categoryRepository.GetCategoryById(Id);
            //if (ExistingCategory == null)
            //    return NotFound();


            //return Ok(ExistingCategory);
            //-------------------------- version 2

            var category = categoryService.GetCategoryById(Id);
            if (category == null)
                return NotFound(ApiResponse<object>.Failure("Not Found"));


            return Ok(ApiResponse<ShowCategoryDto>.Success(category));
        }

        [HttpPost]
        public ActionResult AddCategory(AddCategoryDto category)
        {
            //---------------- version 1
            //var ExistingCategory = categoryRepository.GetCategoryByName(category.Name);
            //if (ExistingCategory != null)
            //    return BadRequest("This Category is Exist");

            //var Category = new Category();

            //Category.Name = category.Name;

            //categoryRepository.AddCategory(Category);
            //categoryRepository.Save();

            //return CreatedAtAction("GetCategorybyId", new { Id = Category.Id }, Category);

            // ------------------------ version 2

            var Category = categoryService.AddCategory(category);

            if (Category == null)
                return BadRequest(ApiResponse<object>.Failure("Category is already exist"));

            categoryService.Save();

            return CreatedAtAction("GetCategorybyId", new { Id = Category.Id }, ApiResponse<AddCategoryDto>.Success(Category));
        }

        [HttpPut]
        public ActionResult EditCategory(int Id, EditCategoryDto category)
        {
            var Category = categoryService.EditCategory(Id, category);

            if (Category == null)
                return NotFound(ApiResponse<object>.Failure("No Found"));

            categoryService.Save();

            return Ok(ApiResponse<ShowCategoryDto>.Success(Category));
        }

        [HttpDelete]
        public ActionResult DeleteCategory(int Id)
        {
            var Category = categoryService.DeleteCategory(Id);

            if (Category == null)
                return NotFound(ApiResponse<object>.Failure("No Found"));

            categoryService.Save();

            return Ok(ApiResponse<object>.Success(null, "Deleted Successfully")); ;
        }

    }
}
