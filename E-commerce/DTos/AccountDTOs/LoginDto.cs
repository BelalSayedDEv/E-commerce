using System.ComponentModel.DataAnnotations;

namespace Assinments.DTos.AccountDTOs
{
    public class LoginDto
    {
        [Required]
        public string UserName { get; set; }
        [DataType(DataType.Password)]
        [Required]
        [StringLength(20, MinimumLength = 4)]
        public string Password { get; set; }

    }
}
