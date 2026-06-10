using System.ComponentModel.DataAnnotations;

namespace Assinments.DTos
{
    public class AddCategoryDto
    {

        public int Id { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string Name { get; set; }
    }
}
