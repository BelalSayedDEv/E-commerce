namespace E_commerce.DTos.Cart
{
    public class CartDto
    {
        public ICollection<CartItemDto> Products { get; set; } = new List<CartItemDto>();

        public double TotalPrice { get; set; }
    }
}
