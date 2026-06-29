using E_Commerce.Model;

namespace E_Commerce.Repository
{
    public interface ICommentRepository
    {
        public Task AddComment(Comment Comment);
        public Task<List<Comment>> FindByProductId(int ProductId);
        public Task<Comment?> FindCommentById(int id, string UserId);
        public void DeleteComment(Comment comment);
        public Task Save();
        public Task<Comment?> FindCommentById(int Id);
        public Task<List<Comment>> GetHistory(string UserId);
    }
}
