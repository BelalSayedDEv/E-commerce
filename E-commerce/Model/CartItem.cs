using System.ComponentModel.DataAnnotations.Schema;

namespace Assinments.Model
{
    public class CartItem
    {
        public int Id { get; set; }
        public int ProductID { get; set; }
        public Product Product { get; set; }

        [ForeignKey("Cart")]
        public int CartId { get; set; }
        public Cart Cart { get; set; }
        public int Quantity { get; set; }
    }
}
