namespace E_commerce.DTos.ProductDTOs
{
    public class ShowProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Price { get; set; }
        public int Quantity { get; set; }
        public int CategoryID { get; set; }
    }
}
