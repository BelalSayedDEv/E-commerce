namespace Assinments.DTos
{
    public class ShowCategoryDto
    {
        public string Name { get; set; }

        public List<string>? ProductsName { get; set; } = new List<string>();
    }
}
