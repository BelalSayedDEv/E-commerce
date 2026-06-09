using System.ComponentModel.DataAnnotations;

namespace Assinments.Model
{
    public class Product
    {
        public int Id { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 2)]

        public string Name { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 10)]
        public string Description { get; set; }

        [Range(minimum: 10, maximum: int.MaxValue)]
        public int Price { get; set; }
        [Range(minimum: 0, maximum: 200)]
        public int Quantity { get; set; }

        [Required]
        public int CategoryID { get; set; }
        public Category? Category { get; set; }

        public OrderItem OrderItem { get; set; }
    }
}
