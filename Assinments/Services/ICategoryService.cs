using Assinments.DTos;
using Assinments.DTos.CategoryDTOs;

namespace Assinments.Services
{
    public interface ICategoryService
    {
        public IEnumerable<ShowCategoryDto>? GetCategories();
        public ShowCategoryDto? GetCategoryById(int Id);
        public ShowCategoryDto? GetCategoryByName(string Name);

        public AddCategoryDto? AddCategory(AddCategoryDto categoryDto);

        public ShowCategoryDto? EditCategory(int Id, EditCategoryDto category);
        public int? DeleteCategory(int Id);
        public void Save();
    }
}
