using Assinments.Model;
using Microsoft.EntityFrameworkCore;

namespace Assinments.Repository
{
    public class CategoryRepostiry : ICategoryRepository
    {
        private readonly ApplicationDbContext Context;

        public CategoryRepostiry(ApplicationDbContext _dbContext)
        {
            Context = _dbContext;
        }
        public List<Category> GetCategories()
        {
            var Categories = Context.Categories.Include(c => c.Products).ToList();
            return Categories;
        }

        public void AddCategory(Category category)
        {
            Context.Categories.Add(category);

        }

        public void Save()
        {
            Context.SaveChanges();
        }

        public Category GetCategoryByName(string Name)
        {
            var category = Context.Categories.FirstOrDefault(c => c.Name.ToLower() == Name.ToLower());

            return category;
        }

        public Category GetCategoryById(int Id)
        {
            var category = Context.Categories.FirstOrDefault(c => c.Id == Id);

            return category;
        }

        public void RemoveCategory(int Id)
        {
            var category = Context.Categories.SingleOrDefault(c => c.Id == Id);
            Context.Remove(category);
        }
    }
}
