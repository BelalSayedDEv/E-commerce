using System.ComponentModel.DataAnnotations;

namespace E_Commerce.DTos.AccountDTOs
{
    public class RefreshDto
    {
        [Required]
        [StringLength(100, MinimumLength = 5)]
        public string Token { get; set; } = string.Empty;


    }
}
