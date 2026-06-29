using E_Commerce.Model;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Repository
{
    public class CommentRepository : ICommentRepository
    {
        private readonly ApplicationDbContext Context;

        public CommentRepository(ApplicationDbContext context)
        {
            this.Context = context;
        }


        public async Task AddComment(Comment Comment)
        {
            await Context.Comments.AddAsync(Comment);
        }


        public async Task<List<Comment>> FindByProductId(int ProductId)
        {
            var comments = await Context.Comments.Where(p => p.ProductId == ProductId).ToListAsync();

            return comments;
        }


        public void DeleteComment(Comment comment)
        {
            Context.Comments.Remove(comment);
        }

        public async Task<Comment?> FindCommentById(int id)
        {
            var Comment = await Context.Comments.FirstOrDefaultAsync
                (c => c.Id == id);
            return Comment;
        }

        public async Task<Comment?> FindCommentById(int id, string UserId)
        {
            var Comment = await Context.Comments.FirstOrDefaultAsync
                (c => c.Id == id && c.UserId == UserId);
            return Comment;
        }

        public async Task Save()
        {
            await Context.SaveChangesAsync();
        }

        public async Task<List<Comment>> GetHistory(string UserId)
        {
            var Comments = await Context.Comments.AsNoTracking().Where(p => p.UserId == UserId).ToListAsync();

            return Comments;
        }
    }
}
