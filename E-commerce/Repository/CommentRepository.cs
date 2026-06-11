using E_Commerce.DTos.CommentDTOs;
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


        public async Task<Comment?> AddComment(Comment Comment)
        {
            var Product = await Context.Products.SingleOrDefaultAsync(p => p.Id == Comment.ProductId);

            if (Product == null)
                return null;

            await Context.AddAsync(Comment);

            return Comment;
        }



        public async Task<List<Comment>?> FindByProductId(int ProductId)
        {
            var comments = await Context.Comments.Where(p => p.ProductId == ProductId).ToListAsync();

            return comments;
        }


        public async Task<bool> DeleteComment(string UserId, int CommentId)
        {
            var Comment = await Context.Comments.SingleOrDefaultAsync
                (c => c.Id == CommentId && c.UserId == UserId);

            if (Comment == null)
                return false;

            Context.Comments.Remove(Comment);

            return true;
        }

        public async Task<Comment?> FindCommentById(string UserId, UpdateCommetDto UpdateCommetDto)
        {
            var Comment = await Context.Comments.SingleOrDefaultAsync
                (c => c.UserId == UserId && c.Id == UpdateCommetDto.CommentId);

            return Comment;
        }

        public async Task Save()
        {
            await Context.SaveChangesAsync();
        }

        public async Task<List<Comment>?> GetHistory(string UserId)
        {
            var Comments = await Context.Comments.Where(p => p.UserId == UserId).ToListAsync();

            return Comments;
        }
    }
}
