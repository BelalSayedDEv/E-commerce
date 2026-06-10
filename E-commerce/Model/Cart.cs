using System.ComponentModel.DataAnnotations.Schema;

namespace E_commerce.Model
{
    public class Cart
    {
        public int Id { get; set; }
        public double TotalPrice { get; set; }
        [ForeignKey("User")]
        public string UserID { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;
        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    }
}
