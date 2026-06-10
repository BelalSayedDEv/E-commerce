using Assinments.DTos.AccountDTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Assinments.Controllers
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
                return Ok("Role Created");

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return BadRequest(ModelState);
        }
    }

}
