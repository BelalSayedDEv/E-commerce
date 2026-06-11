using System.ComponentModel.DataAnnotations;

namespace E_Commerce.DTos.AccountDTOs
{
    public class LoginDto
    {
        [Required]
        [StringLength(20, MinimumLength = 2)]
        public string UserName { get; set; } = string.Empty;
        [DataType(DataType.Password)]
        [Required]
        [StringLength(20, MinimumLength = 4)]
        public string Password { get; set; } = string.Empty;

    }
}
