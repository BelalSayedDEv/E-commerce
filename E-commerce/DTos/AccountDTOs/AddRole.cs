using System.ComponentModel.DataAnnotations;

namespace E_commerce.DTos.AccountDTOs
{
    public class AddRole
    {
        [Required]
        [StringLength(20, MinimumLength = 2)]
        public string RoleName { get; set; } = string.Empty;
    }
}
