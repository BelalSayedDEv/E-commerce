using System.ComponentModel.DataAnnotations.Schema;

namespace E_Commerce.Model
{
    public class OrderItem
    {
        public int Id { get; set; }
        [ForeignKey("Order")]
        public int OrderId { get; set; }
        [ForeignKey("Product")]
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
        public Order Order { get; set; } = null!;
        public int Quantity { get; set; }
        public int Price { get; set; }
    }
}
