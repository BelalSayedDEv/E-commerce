using Assinments.Model;

namespace Assinments.Repository
{
    public interface ICategoryRepository
    {

        public List<Category> GetCategories();

        public void Save();

        public void AddCategory(Category category);

        public void RemoveCategory(int Id);

        public Category GetCategoryByName(string Name);

        public Category GetCategoryById(int Id);

    }
}
