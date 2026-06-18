using E_Commerce.DTos.CommentDTOs;

namespace E_Commerce.Services
{
    public interface ICommentService
    {
        public Task<ShowCommentDto?> Add(string UserName, string UserId, AddCommentDto addCommentDto);
        public Task<CommentHistoryDto?> FindByProdcutId(int ProductId);
        public Task<ShowCommentDto?> UpdateComment(string UserId, UpdateCommetDto updateCommetDto);
        public Task<bool> DeleteComment(string Role, string UserId, int CommentId);

        public Task<CommentHistoryDto?> GetHistoryOfCommentByUserId(string UserId);

    }
}
