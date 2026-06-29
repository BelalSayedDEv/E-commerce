using System.ComponentModel.DataAnnotations.Schema;

namespace E_Commerce.Model
{
    public class CartItem
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public int ProductID { get; set; }
        public Product Product { get; set; } = null!;

        [ForeignKey("Cart")]
        public int CartId { get; set; }
        public Cart Cart { get; set; } = null!;

    }
}
