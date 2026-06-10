using E_commerce.DTos.AccountDTOs;
using E_commerce.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace E_commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> roleManager;

        public RoleController(RoleManager<IdentityRole> roleManager)
        {
            this.roleManager = roleManager;
        }


        [HttpPost("AddRole")]

        public async Task<IActionResult> AddRole(AddRole RoleDto)
        {
            var Role = new IdentityRole()
            {

                Name = RoleDto.RoleName
            };

            var result = await roleManager.CreateAsync(Role);

            if (result.Succeeded)
                return Ok(ApiResponse<Object>.Success(null, "Role Created"));

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            var Errors = ModelState.Values.SelectMany(x => x.Errors)
                                         .Select(x => x.ErrorMessage)
                                         .ToList();

            return BadRequest(ApiResponse<object>.Failure("Validation Faild", Errors));
        }
    }

}
