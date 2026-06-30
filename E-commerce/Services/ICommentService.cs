using E_Commerce.DTos.CommentDTOs;

namespace E_Commerce.Services
{
    public interface ICommentService
    {
        public Task<ShowCommentDto?> Add(string UserName, string UserId, AddCommentDto addCommentDto);
        public Task<CommentHistoryDto> FindByProductId(int ProductId);
        public Task<ShowCommentDto?> UpdateComment(string UserId, UpdateCommetDto updateCommetDto);
        public Task<bool> DeleteAnyComment(int CommentId);

        public Task<bool> DeleteOwnComment(string UserId, int CommentId);
        public Task<CommentHistoryDto> GetHistoryOfCommentByUserId(string UserId);

    }
}
