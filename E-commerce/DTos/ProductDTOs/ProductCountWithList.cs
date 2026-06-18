namespace E_Commerce.DTos.ProductDTOs
{
    public class ProductCountWithList
    {
        public int TotalCount { get; set; }

        public List<ShowProductDto>? ProductList { get; set; } = new List<ShowProductDto>();
    }
}
