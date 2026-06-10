using System.ComponentModel.DataAnnotations;

namespace E_commerce.DTos.AccountDTOs
{
    public class RegisterDto
    {
        [Required]
        [StringLength(20, MinimumLength = 2)]
        public string UserName { get; set; } = string.Empty;
        [Required]
        [StringLength(20, MinimumLength = 2)]
        public string FullName { get; set; } = string.Empty;
        [Required]
        [StringLength(20, MinimumLength = 2)]
        public string Address { get; set; } = string.Empty;
        [DataType(DataType.Password)]
        [Required]
        [StringLength(20, MinimumLength = 4)]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Required]
        [StringLength(20, MinimumLength = 4)]
        [Compare("Password")]
        public string ConfirmPassword { get; set; } = string.Empty;
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = string.Empty;
    }
}
