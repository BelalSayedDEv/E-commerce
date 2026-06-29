using E_Commerce.DTos;
using E_Commerce.DTos.CategoryDTOs;
using E_Commerce.Model;
using E_Commerce.Services;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Controllers
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
        public ActionResult<IEnumerable<ShowCategoryDto>> GetAllCategory()
        {

            var categories = categoryService.GetCategories();

            return Ok(ApiResponse<IEnumerable<ShowCategoryDto>>.Success(categories));
        }

        [HttpGet("{Id}")]
        public ActionResult<ShowCategoryDto> GetCategorybyId(int Id)
        {

            var category = categoryService.GetCategoryById(Id);

            if (category == null)
                return NotFound(ApiResponse<object>.Failure("Not Found"));


            return Ok(ApiResponse<ShowCategoryDto>.Success(category));
        }

        [HttpPost]
        public ActionResult AddCategory(AddCategoryDto category)
        {

            var Category = categoryService.AddCategory(category);

            if (Category == null)
                return Conflict(ApiResponse<object>.Failure("Category is already exist"));

            categoryService.Save();

            return CreatedAtAction("GetCategorybyId", new { Id = Category.Id }, ApiResponse<AddCategoryDto>.Success(Category));
        }

        [HttpPut]
        public ActionResult EditCategory(int Id, EditCategoryDto category)
        {
            if (Id <= 0)
                return BadRequest("Less than zero");

            var Category = categoryService.EditCategory(Id, category);

            if (Category == null)
                return NotFound(ApiResponse<object>.Failure("No Found"));

            categoryService.Save();

            return Ok(ApiResponse<ShowCategoryDto>.Success(Category));
        }

        [HttpDelete("{Id}")]
        public ActionResult DeleteCategory(int Id)
        {
            if (Id <= 0)
                return BadRequest("Less than zero");

            var Category = categoryService.DeleteCategory(Id);

            if (Category == null)
                return NotFound(ApiResponse<object>.Failure("No Found"));

            categoryService.Save();

            return NoContent(); ;
        }

    }
}
