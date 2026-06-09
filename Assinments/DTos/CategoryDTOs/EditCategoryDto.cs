using System.ComponentModel.DataAnnotations;

namespace Assinments.DTos.CategoryDTOs
{
    public class EditCategoryDto
    {
        [Required]
        [StringLength(20, MinimumLength = 3)]
        [Display(Name = "Category Name")]
        public string Name { get; set; }
    }
}
