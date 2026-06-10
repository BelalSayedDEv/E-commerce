using System.ComponentModel.DataAnnotations.Schema;

namespace Assinments.Model
{
    public class Order
    {
        public int Id { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public DateTime OrderDate { get; set; }

        public double TotalOrderPrice { get; set; }

        public string Status { get; set; }

        public ICollection<OrderItem> Items { get; set; }
    }
}
