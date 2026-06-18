namespace E_Commerce.DTos.CommentDTOs
{
    public class ShowCommentDto
    {
        public int Id { get; set; }

        public string Comment { get; set; } = string.Empty;
        public string username { get; set; } = string.Empty;
        public int ProductId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
