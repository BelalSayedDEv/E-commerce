using E_Commerce.Contracts;
using E_Commerce.DTos.AccountDTOs;
using E_Commerce.Model;
using E_Commerce.Repository;
using E_Commerce.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace E_Commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService accountService;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IConfiguration configuration;
        private readonly ICartRepository cartRepository;

        public AccountController(IAccountService accountService, UserManager<ApplicationUser> userManager, IConfiguration configuration, ICartRepository cartRepository)
        {
            this.accountService = accountService;
            this.userManager = userManager;
            this.configuration = configuration;
            this.cartRepository = cartRepository;
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync(LoginDto UserFromReq)
        {
            var result = await accountService.Login(UserFromReq);

            if (result.outcome == AccountOutcome.Unauthorized)
                return Unauthorized(ApiResponse<object>.Failure(result.Message!));

            return Ok(ApiResponse<ResponseTokenDTO>.Success(result.Data));
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(RefreshDto refreshDto)
        {
            var userid = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var result = await accountService.Refresh(userid, refreshDto);

            switch (result.outcome)
            {
                case AccountOutcome.TokenNotFound:
                    return NotFound(ApiResponse<object>.Failure(result.Message!));

                case AccountOutcome.UserNotOwnToken:
                    return NotFound(ApiResponse<object>.Failure(result.Message!)); ;

                case AccountOutcome.TokenExpired:
                    return Conflict(ApiResponse<object>.Failure(result.Message!)); ;

                case AccountOutcome.TokenRevoked:
                    return Conflict(ApiResponse<object>.Failure(result.Message!)); ;

                case AccountOutcome.TokenUsed:
                    return Conflict(ApiResponse<object>.Failure(result.Message!)); ;
            }

            return Ok(ApiResponse<ResponseTokenDTO>.Success(result.Data));

        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto UserFromReq)
        {
            if (ModelState.IsValid)
            {
                var userFromDb = await userManager.FindByNameAsync(UserFromReq.UserName);
                if (userFromDb != null)
                    return Conflict(ApiResponse<object>.Failure("User is Exist"));

                var user = new ApplicationUser
                {
                    UserName = UserFromReq.UserName,
                    FullName = UserFromReq.FullName,
                    Email = UserFromReq.Email,
                    Address = UserFromReq.Address

                };
                var result = await userManager.CreateAsync(user, UserFromReq.Password);

                if (result.Succeeded)
                {
                    var cart = new Cart { UserID = user.Id };
                    cartRepository.AddCart(cart);
                    cartRepository.Save();

                    return Created("", ApiResponse<object>.Success(null, "Created"));
                }

                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("Password", "Password Or User In");

                }

            }

            var Errors = ModelState.Values.SelectMany(x => x.Errors)
                               .Select(x => x.ErrorMessage)
                               .ToList();


            return BadRequest(ApiResponse<object>.Failure("Validation Faild", Errors));

        }


        [HttpPost("register-admin")]
        public async Task<IActionResult> AdminRegister(RegisterDto UserFromReq)
        {
            var user = await userManager.FindByNameAsync(UserFromReq.UserName);

            if (user != null)
                return Conflict(ApiResponse<object>.Failure("User Is Exist"));

            var User = new ApplicationUser
            {
                UserName = UserFromReq.UserName,
                Email = UserFromReq.Email,
                FullName = UserFromReq.FullName,
                Address = UserFromReq.Address,

            };

            var result = await userManager.CreateAsync(User, UserFromReq.Password);

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(User, "Admin");
                return Created("", ApiResponse<object>.Success(null, "Created"));
            }

            foreach (var item in result.Errors)
            {
                ModelState.AddModelError("Password", "Password Or User In");

            }

            var Errors = ModelState.Values.SelectMany(x => x.Errors)
                                         .Select(x => x.ErrorMessage)
                                         .ToList();


            return BadRequest(ApiResponse<object>.Failure("Validation Faild", Errors));
        }
    }
}
