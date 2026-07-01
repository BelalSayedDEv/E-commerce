using E_Commerce.Contracts;
using E_Commerce.DTos.AccountDTOs;
using E_Commerce.Model;
using E_Commerce.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace E_Commerce.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IAccountRepository accountRepository;
        private readonly IConfiguration configuration;

        public AccountService(UserManager<ApplicationUser> userManager, IAccountRepository accountRepository, IConfiguration configuration)
        {
            this.userManager = userManager;
            this.accountRepository = accountRepository;
            this.configuration = configuration;
        }

        private string GenerateRefreshToken()
        {
            return Convert.ToHexString(
                RandomNumberGenerator.GetBytes(64));
        }
        public async Task<AccountResult> Login(LoginDto UserFromReq)
        {

            ApplicationUser? user = await userManager.FindByNameAsync(UserFromReq.UserName);

            if (user == null)
                return new AccountResult()
                {
                    outcome = AccountOutcome.Unauthorized,
                    Message = "Username or password wrong"
                };

            var Found = await userManager.CheckPasswordAsync(user, UserFromReq.Password);

            if (!Found)
            {
                return new AccountResult()
                {
                    outcome = AccountOutcome.Unauthorized,
                    Message = "Username or password wrong"
                };
            }
            // Generte Claims 

            List<Claim> claims = new List<Claim>();

            claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));
            claims.Add(new Claim(ClaimTypes.Name, user.FullName));

            var Roles = await userManager.GetRolesAsync(user);

            foreach (var Role in Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, Role));
            }

            //create symatric Key


            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:secret"]!));


            // create Signing Caradentials

            SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken token = new JwtSecurityToken(
                issuer: configuration["Jwt:issuer"],
                audience: configuration["Jwt:audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(15),
                signingCredentials: credentials
                );


            RefreshToken refreshToken = new RefreshToken
            {
                Token = GenerateRefreshToken(),
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                UserId = user.Id,
            };


            await accountRepository.AddRefreshToken(refreshToken);
            await accountRepository.Save();

            ResponseTokenDTO FinalToken = new ResponseTokenDTO
            {
                RefreshToken = refreshToken.Token,
                AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = DateTime.UtcNow.AddMinutes(15),

            };

            return new AccountResult()
            {
                Data = FinalToken,
                outcome = AccountOutcome.Authorized
            };
        }

        public async Task<AccountResult> Refresh(string UserId, RefreshDto refreshDto)
        {
            var Token = await accountRepository.GetTokenByToken(refreshDto.Token);

            if (Token is null)
                return new AccountResult()
                {
                    outcome = AccountOutcome.TokenNotFound,
                    Message = "Token Not Found"
                };

            if (Token.UserId != UserId)
                return new AccountResult()
                {
                    outcome = AccountOutcome.UserNotOwnToken,
                    Message = "Token Not Found" // resource hidding

                };
            if (Token.IsUsed == true)
                return new AccountResult()
                {
                    outcome = AccountOutcome.TokenUsed,
                    Message = "Token is used" // resource hidding

                };
            if (Token.IsRevoked == true)
                return new AccountResult()
                {
                    outcome = AccountOutcome.TokenRevoked,
                    Message = "Token is Revoked" // resource hidding

                };

            if (DateTime.UtcNow > Token.ExpiresAt)
                return new AccountResult()
                {
                    outcome = AccountOutcome.TokenExpired,
                    Message = "Token is Expired" // resource hidding

                };

            Token.IsRevoked = true;
            Token.IsUsed = true;

            var user = await userManager.FindByIdAsync(UserId);

            // Generte Claims 

            List<Claim> claims = new List<Claim>();

            claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));

            claims.Add(new Claim(ClaimTypes.Name, user.FullName));

            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));

            var Roles = await userManager.GetRolesAsync(user);

            foreach (var Role in Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, Role));
            }

            //create symatric Key


            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:secret"]!));


            // create Signing Caradentials

            SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken token = new JwtSecurityToken(
                issuer: configuration["Jwt:issuer"],
                audience: configuration["Jwt:audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(15),
                signingCredentials: credentials
                );


            RefreshToken refreshToken = new RefreshToken
            {
                Token = GenerateRefreshToken(),
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                UserId = user.Id,
            };


            await accountRepository.AddRefreshToken(refreshToken);
            await accountRepository.Save();

            ResponseTokenDTO FinalToken = new ResponseTokenDTO
            {
                RefreshToken = refreshToken.Token,
                AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = DateTime.UtcNow.AddMinutes(15),

            };

            return new AccountResult()
            {
                Data = FinalToken,
                outcome = AccountOutcome.Authorized
            };

        }
    }
}
