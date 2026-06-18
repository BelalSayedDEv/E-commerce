using E_Commerce.DTos.CommentDTOs;
using E_Commerce.Model;

namespace E_Commerce.Repository
{
    public interface ICommentRepository
    {
        public Task<Comment?> AddComment(Comment Comment);
        public Task<List<Comment>?> FindByProductId(int ProductId);
        public Task<bool> DeleteComment(string Role, string UserID, int CommentId);
        public Task Save();
        public Task<Comment?> FindCommentById(string UserID, UpdateCommetDto updateCommetDto);
        public Task<List<Comment>?> GetHistory(string UserId);
    }
}
