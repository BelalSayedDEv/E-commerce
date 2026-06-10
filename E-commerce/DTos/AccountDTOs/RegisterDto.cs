using System.ComponentModel.DataAnnotations;

namespace Assinments.DTos.AccountDTOs
{
    public class RegisterDto
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        [StringLength(20, MinimumLength = 2)]
        public string FullName { get; set; }
        [Required]
        [StringLength(20, MinimumLength = 2)]
        public string Address { get; set; }
        [DataType(DataType.Password)]
        [Required]
        [StringLength(20, MinimumLength = 4)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Required]
        [StringLength(20, MinimumLength = 4)]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
    }
}
