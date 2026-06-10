using System.ComponentModel.DataAnnotations.Schema;

namespace Assinments.Model
{
    public class Cart
    {
        public int Id { get; set; }
        public double TotalPrice { get; set; }
        [ForeignKey("User")]
        public string UserID { get; set; }
        public ApplicationUser User { get; set; }
        public ICollection<CartItem> CartItems { get; set; }
    }
}
