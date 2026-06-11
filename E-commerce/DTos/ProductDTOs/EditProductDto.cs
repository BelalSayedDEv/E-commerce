using System.ComponentModel.DataAnnotations;

namespace E_Commerce.DTos.ProductDTOs
{
    public class EditProductDto
    {
        [Required]
        [StringLength(50, MinimumLength = 2)]

        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 10)]
        public string Description { get; set; } = string.Empty;

        [Range(minimum: 10, maximum: int.MaxValue)]
        public int Price { get; set; }
        [Range(minimum: 0, maximum: 200)]
        public int Quantity { get; set; }

        [Required]
        public int CategoryID { get; set; }
    }
}
