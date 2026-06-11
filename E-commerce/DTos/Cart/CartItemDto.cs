namespace E_Commerce.DTos.Cart
{
    public class CartItemDto
    {
        public int Id { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public double Price { get; set; }
        public int Quantity { get; set; }
        public double SubTotal { get; set; }

    }

}
