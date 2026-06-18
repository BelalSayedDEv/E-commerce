namespace E_Commerce.DTos.ProductDTOs
{
    public class ProductFilterDto
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchTerm { get; set; }           // filter by name
        public string? SortBy { get; set; } = "Id";       // column name
        public SortDirection SortDirection { get; set; } = SortDirection.Ascending;
    }

    public enum SortDirection { Ascending, Descending }
}
