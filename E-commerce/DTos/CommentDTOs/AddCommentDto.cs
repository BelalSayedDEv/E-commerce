using System.ComponentModel.DataAnnotations;

namespace E_Commerce.DTos.CommentDTOs
{
    public class AddCommentDto
    {
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public int ProductId { get; set; }

    }
}
