namespace E_Commerce.DTos
{
    public class ShowCategoryDto
    {
        public string Name { get; set; } = string.Empty;

        public List<string> ProductsName { get; set; } = new List<string>();
    }
}
