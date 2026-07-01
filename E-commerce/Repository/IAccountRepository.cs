using E_Commerce.Model;

namespace E_Commerce.Repository
{
    public interface IAccountRepository
    {
        public Task AddRefreshToken(RefreshToken refreshToken);
        public Task<RefreshToken?> GetTokenByToken(string Token);
        public Task Save();
    }
}
