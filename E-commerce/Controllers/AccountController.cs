using E_Commerce.DTos.AccountDTOs;
using E_Commerce.Model;
using E_Commerce.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace E_Commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IConfiguration configuration;
        private readonly ICartRepository cartRepository;

        public AccountController(UserManager<ApplicationUser> userManager, IConfiguration configuration, ICartRepository cartRepository)
        {
            this.userManager = userManager;
            this.configuration = configuration;
            this.cartRepository = cartRepository;
        }


        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterDto UserFromReq)
        {
            if (ModelState.IsValid)
            {
                var userFromDb = await userManager.FindByNameAsync(UserFromReq.UserName);
                if (userFromDb != null)
                    return BadRequest(ApiResponse<object>.Failure("User is Exist"));

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

                    return Ok(ApiResponse<object>.Success(null, "Created"));
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

        [HttpPost("Login")]
        public async Task<IActionResult> LoginAsync(LoginDto UserFromReq)
        {
            if (!ModelState.IsValid)
            {
                var Errors1 = ModelState.Values.SelectMany(x => x.Errors)
                               .Select(x => x.ErrorMessage)
                               .ToList();


                return BadRequest(ApiResponse<object>.Failure("Validation Faild", Errors1));
            }


            var user = await userManager.FindByNameAsync(UserFromReq.UserName);

            if (user != null)
            {
                bool Found = await userManager.CheckPasswordAsync(user, UserFromReq.Password);

                if (Found)
                {
                    //generate Token


                    List<Claim> claims = new List<Claim>();

                    claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));
                    claims.Add(new Claim(ClaimTypes.Name, user.UserName!));
                    claims.Add(new Claim(CustomClaims.FullName, user.FullName));

                    // generated jti  Id token 

                    claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));

                    var Role = await userManager.GetRolesAsync(user);


                    foreach (var item in Role)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, item));
                    }

                    // finish claims 

                    var SignIn = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Secrit"]!));

                    var signingKey = new SigningCredentials(SignIn, SecurityAlgorithms.HmacSha256);



                    //design Token Response

                    JwtSecurityToken token = new JwtSecurityToken(
                        issuer: configuration["Jwt:Issu"],
                        audience: configuration["Jwt:Aud"],
                        expires: DateTime.Now.AddDays(1),
                        claims: claims,
                        signingCredentials: signingKey
                        );

                    object result_Token = new
                    {
                        token = new JwtSecurityTokenHandler().WriteToken(token),
                        expiration = DateTime.Now.AddDays(1),
                    };

                    return Ok(ApiResponse<object>.Success(result_Token));
                }

            }

            ModelState.AddModelError("Password", "Password or Username is Invalid");

            var Errors = ModelState.Values.SelectMany(x => x.Errors)
                                           .Select(x => x.ErrorMessage)
                                           .ToList();


            return BadRequest(ApiResponse<object>.Failure("Validation Faild", Errors));
        }




        [HttpPost("AdminRegister")]
        public async Task<IActionResult> AdminRegister(RegisterDto UserFromReq)
        {
            var user = await userManager.FindByNameAsync(UserFromReq.UserName);

            if (user != null)
                return BadRequest(ApiResponse<object>.Failure("User Is Exist"));

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
                return Ok(ApiResponse<object>.Success(null, "Created"));
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
