using E_Commerce.Contracts;
using E_Commerce.DTos.AccountDTOs;

namespace E_Commerce.Services
{
    public interface IAccountService
    {
        public Task<AccountResult> Login(LoginDto UserFromReq);
        public Task<AccountResult> Refresh(string UserId, RefreshDto refreshDto);
    }
}
