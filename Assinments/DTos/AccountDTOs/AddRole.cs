using System.ComponentModel.DataAnnotations;

namespace Assinments.DTos.AccountDTOs
{
    public class AddRole
    {
        [Required]
        [StringLength(20, MinimumLength = 2)]
        public string RoleName { get; set; }
    }
}
