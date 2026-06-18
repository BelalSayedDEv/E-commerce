using System.ComponentModel.DataAnnotations;

namespace E_Commerce.DTos.ProfileDtos
{
    public class UpdateProfileDto
    {
        [StringLength(20, MinimumLength = 3)]
        public string? FullName { get; set; }

        [StringLength(20, MinimumLength = 3)]
        public string? Address { get; set; }
        [StringLength(20, MinimumLength = 3)]
        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }
    }
}
