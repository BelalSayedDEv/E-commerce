using E_Commerce.Model;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Repository
{
    public class AccountRepository : IAccountRepository
    {
        private readonly ApplicationDbContext context;

        public AccountRepository(ApplicationDbContext context)
        {
            this.context = context;
        }
        public async Task AddRefreshToken(RefreshToken refreshToken)
        {
            await context.RefreshTokens.AddAsync(refreshToken);
        }

        public async Task<RefreshToken?> GetTokenByToken(string Token)
        {
            var token = await context.RefreshTokens.FirstOrDefaultAsync(r => r.Token == Token);
            return token;
        }

        public async Task Save()
        {
            await context.SaveChangesAsync();
        }
    }
}
