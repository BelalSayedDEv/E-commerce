using System.ComponentModel.DataAnnotations;

namespace E_commerce.DTos.CategoryDTOs
{
    public class EditCategoryDto
    {
        [Required]
        [StringLength(20, MinimumLength = 3)]
        [Display(Name = "Category Name")]
        public string Name { get; set; } = string.Empty;
    }
}
