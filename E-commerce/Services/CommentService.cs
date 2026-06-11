using E_Commerce.DTos.CommentDTOs;
using E_Commerce.Model;
using E_Commerce.Repository;

namespace E_Commerce.Services
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository commentRepository;

        public CommentService(ICommentRepository commentRepository)
        {
            this.commentRepository = commentRepository;
        }
        public async Task<ShowCommentDto?> Add(string UserId, AddCommentDto addCommentDto)
        {
            Comment comment = new Comment()
            {
                Description = addCommentDto.Description,
                ProductId = addCommentDto.ProductId,
                UserId = UserId,
                CreatedAt = DateTime.Now
            };

            var Comment = await commentRepository.AddComment(comment);

            await commentRepository.Save();

            if (Comment == null)
                return null;

            ShowCommentDto showCommentDto = new ShowCommentDto();

            showCommentDto.Id = Comment.Id;
            showCommentDto.Comment = Comment.Description;
            showCommentDto.ProductId = Comment.ProductId;
            showCommentDto.CreatedAt = Comment.CreatedAt;
            return showCommentDto;

        }


        public async Task<CommentHistoryDto?> FindByProdcutId(int ProductId)
        {
            var Comments = await commentRepository.FindByProductId(ProductId);

            if (Comments == null)
                return null;

            CommentHistoryDto commentHistoryDto = new CommentHistoryDto();

            foreach (var Comment in Comments)
            {
                ShowCommentDto commentDto = new ShowCommentDto();
                commentDto.Id = Comment.Id;
                commentDto.Comment = Comment.Description;
                commentDto.ProductId = Comment.ProductId;
                commentDto.CreatedAt = Comment.CreatedAt;
                commentHistoryDto.Comments.Add(commentDto);

            }

            return commentHistoryDto;
        }

        public async Task<CommentHistoryDto?> GetHistoryOfCommentByUserId(string UserId)
        {
            var Comments = await commentRepository.GetHistory(UserId);

            if (Comments == null)
                return null;

            CommentHistoryDto commentHistoryDto = new CommentHistoryDto();

            foreach (var Comment in Comments)
            {
                ShowCommentDto commentDto = new ShowCommentDto();
                commentDto.Id = Comment.Id;
                commentDto.Comment = Comment.Description;
                commentDto.ProductId = Comment.ProductId;
                commentDto.CreatedAt = Comment.CreatedAt;

                commentHistoryDto.Comments.Add(commentDto);

            }
            return commentHistoryDto;

        }

        public async Task<ShowCommentDto?> UpdateComment(string UserId, UpdateCommetDto updateCommetDto)
        {
            var comment = await commentRepository.FindCommentById(UserId, updateCommetDto);

            if (comment == null)
                return null;

            comment.Description = updateCommetDto.CommentText;

            ShowCommentDto commentDto = new ShowCommentDto();

            commentDto.Id = comment.Id;
            commentDto.Comment = comment.Description;
            commentDto.ProductId = comment.ProductId;
            commentDto.CreatedAt = comment.CreatedAt;

            await commentRepository.Save();

            return commentDto;

        }

        public async Task<bool> DeleteComment(string UserId, int CommentId)
        {
            var result = await commentRepository.DeleteComment(UserId, CommentId);

            if (result)
                await commentRepository.Save();
            return result;
        }

    }
}
