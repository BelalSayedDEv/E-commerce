using E_commerce.DTos;
using E_commerce.DTos.CategoryDTOs;
using E_commerce.Model;
using E_commerce.Repository;

namespace E_commerce.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            this.categoryRepository = categoryRepository;
        }

        public IEnumerable<ShowCategoryDto>? GetCategories()
        {
            var Categories = categoryRepository.GetCategories();

            if (Categories == null)
                return null;


            List<ShowCategoryDto> list = new List<ShowCategoryDto>();

            foreach (var category in Categories)
            {
                ShowCategoryDto showCategoryDto = new ShowCategoryDto();

                showCategoryDto.Name = category.Name;

                foreach (var product in category.Products)
                {
                    showCategoryDto.ProductsName.Add(product.Name);
                }

                list.Add(showCategoryDto);
            }


            return list;
        }


        public ShowCategoryDto? GetCategoryById(int Id)
        {
            var Category = categoryRepository.GetCategoryById(Id);
            if (Category == null)
                return null; //NotFound

            ShowCategoryDto result = new ShowCategoryDto();

            result.Name = Category.Name;

            return result;
        }


        public ShowCategoryDto? GetCategoryByName(string Name)
        {
            var Category = categoryRepository.GetCategoryByName(Name);
            if (Category == null)
                return null; // NotFound

            ShowCategoryDto result = new ShowCategoryDto();

            result.Name = Category.Name;

            return result;
        }



        public ShowCategoryDto? EditCategory(int Id, EditCategoryDto category)
        {
            var Category = categoryRepository.GetCategoryById(Id);

            if (Category == null)
                return null; //NotFound

            Category.Name = category.Name;

            ShowCategoryDto result = new ShowCategoryDto() { Name = category.Name };

            return result;
        }



        public int? DeleteCategory(int Id)
        {
            var category = categoryRepository.GetCategoryById(Id);
            if (category == null)
                return null; //NotFound

            categoryRepository.RemoveCategory(Id);
            return 1;

        }
        public void Save()
        {
            categoryRepository.Save();
        }

        public AddCategoryDto? AddCategory(AddCategoryDto categoryDto)
        {
            var existingCategory = categoryRepository.GetCategoryByName(categoryDto.Name);
            if (existingCategory != null)
                return null;//BadRequest

            Category category = new Category();
            category.Name = categoryDto.Name;

            categoryRepository.AddCategory(category);
            categoryRepository.Save();

            categoryDto.Id = category.Id;
            return categoryDto;
        }
    }
}
