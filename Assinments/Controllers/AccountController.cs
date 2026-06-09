using Assinments.DTos.AccountDTOs;
using Assinments.Model;
using Assinments.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Assinments.Controllers
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
                    return BadRequest(ModelState);

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

                    return Ok("Created");
                }

                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("Password", "Password Or User In");

                }

            }

            return BadRequest(ModelState);

        }

        [HttpPost("Login")]
        public async Task<IActionResult> LoginAsync(LoginDto UserFromReq)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await userManager.FindByNameAsync(UserFromReq.UserName);

            if (user != null)
            {
                bool Found = await userManager.CheckPasswordAsync(user, UserFromReq.Password);

                if (Found)
                {
                    //generate Token


                    List<Claim> claims = new List<Claim>();

                    claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));
                    claims.Add(new Claim(ClaimTypes.Name, user.UserName));

                    // generated jti  Id token 

                    claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));

                    var Role = await userManager.GetRolesAsync(user);


                    foreach (var item in Role)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, item));
                    }

                    // finish claims 

                    var SignIn = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Secrit"]));

                    var signingKey = new SigningCredentials(SignIn, SecurityAlgorithms.HmacSha256);



                    //design Token Response

                    JwtSecurityToken token = new JwtSecurityToken(
                        issuer: configuration["Jwt:Issu"],
                        audience: configuration["Jwt:Aud"],
                        expires: DateTime.Now.AddDays(1),
                        claims: claims,
                        signingCredentials: signingKey
                        );


                    return Ok(new
                    {
                        token = new JwtSecurityTokenHandler().WriteToken(token),
                        expiration = DateTime.Now.AddDays(1),
                    });
                }

            }

            ModelState.AddModelError("Password", "Password or Username is Invalid");
            return BadRequest(ModelState);
        }

        [HttpPost("AdminRegister")]
        public async Task<IActionResult> AdminRegister(RegisterDto UserFromReq)
        {
            var user = await userManager.FindByNameAsync(UserFromReq.UserName);

            if (user != null)
                return BadRequest("User Is Exist");

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
                return Ok("Created");
            }

            foreach (var item in result.Errors)
            {
                ModelState.AddModelError("Password", "Password Or User In");

            }

            return BadRequest(ModelState);
        }
    }
}
