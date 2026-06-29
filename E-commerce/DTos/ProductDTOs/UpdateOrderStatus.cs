using System.ComponentModel.DataAnnotations;

namespace E_Commerce.DTos.ProductDTOs
{
    public class UpdateOrderStatus
    {
        [Required]
        [StringLength(20, MinimumLength = 4)]
        public string Status { get; set; } = string.Empty;
    }
}
