using E_Commerce.DTos.CommentDTOs;
using E_Commerce.Model;
using E_Commerce.Repository;

namespace E_Commerce.Services
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository commentRepository;
        private readonly IProductRepository productRepository;

        public CommentService(ICommentRepository commentRepository, IProductRepository productRepository)
        {
            this.commentRepository = commentRepository;
            this.productRepository = productRepository;
        }
        public async Task<ShowCommentDto?> Add(string UserName, string UserId, AddCommentDto addCommentDto)
        {

            var product = await productRepository.GetProductByIdAsync(addCommentDto.ProductId);
            if (product == null)
                return null;

            Comment comment = new Comment()
            {
                Description = addCommentDto.Description,
                ProductId = addCommentDto.ProductId,
                UserId = UserId,
                CreatedAt = DateTime.Now,
                Username = UserName,
            };

            await commentRepository.AddComment(comment);

            await commentRepository.Save();


            ShowCommentDto showCommentDto = new ShowCommentDto();

            showCommentDto.Id = comment.Id;
            showCommentDto.UserName = comment.Username;
            showCommentDto.Comment = comment.Description;
            showCommentDto.ProductId = comment.ProductId;
            showCommentDto.CreatedAt = comment.CreatedAt;

            return showCommentDto;
        }


        public async Task<CommentHistoryDto> FindByProductId(int ProductId)
        {
            var Comments = await commentRepository.FindByProductId(ProductId);

            CommentHistoryDto commentHistoryDto = new CommentHistoryDto();

            foreach (var Comment in Comments)
            {
                ShowCommentDto commentDto = new ShowCommentDto();

                commentDto.Id = Comment.Id;
                commentDto.UserName = Comment.Username;
                commentDto.Comment = Comment.Description;
                commentDto.ProductId = Comment.ProductId;
                commentDto.CreatedAt = Comment.CreatedAt;
                commentHistoryDto.Comments.Add(commentDto);

            }

            return commentHistoryDto;
        }

        public async Task<CommentHistoryDto> GetHistoryOfCommentByUserId(string UserId)
        {
            var Comments = await commentRepository.GetHistory(UserId);

            CommentHistoryDto commentHistoryDto = new CommentHistoryDto();

            foreach (var Comment in Comments)
            {
                ShowCommentDto commentDto = new ShowCommentDto();

                commentDto.Id = Comment.Id;
                commentDto.UserName = Comment.Username;
                commentDto.Comment = Comment.Description;
                commentDto.ProductId = Comment.ProductId;
                commentDto.CreatedAt = Comment.CreatedAt;

                commentHistoryDto.Comments.Add(commentDto);

            }
            return commentHistoryDto;

        }

        public async Task<ShowCommentDto?> UpdateComment(string UserId, UpdateCommetDto updateCommetDto)
        {
            var comment = await commentRepository.FindCommentById(updateCommetDto.CommentId, UserId);

            if (comment == null)
                return null;

            comment.Description = updateCommetDto.CommentText;

            ShowCommentDto commentDto = new ShowCommentDto();

            commentDto.Id = comment.Id;
            commentDto.Comment = comment.Description;
            commentDto.ProductId = comment.ProductId;
            commentDto.CreatedAt = comment.CreatedAt;
            commentDto.UserName = comment.Username;

            await commentRepository.Save();

            return commentDto;

        }

        public async Task<bool> DeleteComment(string? Role, string UserId, int CommentId)
        {


            if (Role == "Admin")
            {
                var comment1 = await commentRepository.FindCommentById(CommentId);

                if (comment1 == null)
                    return false;
                commentRepository.DeleteComment(comment1);
                await commentRepository.Save();
                return true;
            }

            var comment = await commentRepository.FindCommentById(CommentId, UserId);

            if (comment != null)
            {
                commentRepository.DeleteComment(comment);
                await commentRepository.Save();
                return true;
            }

            return false;
        }


    }

}

