using System.ComponentModel.DataAnnotations;

namespace E_Commerce.DTos.CommentDTOs
{
    public class UpdateCommetDto
    {
        [Required]
        public int CommentId { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string CommentText { get; set; } = string.Empty;
    }
}
