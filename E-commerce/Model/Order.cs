using System.ComponentModel.DataAnnotations.Schema;

namespace E_Commerce.Model
{
    public class Order
    {
        public int Id { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;

        public DateTime OrderDate { get; set; }

        public double TotalOrderPrice { get; set; }

        public string Status { get; set; } = "Pending";

        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    }
}
