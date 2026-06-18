using E_Commerce.DTos.ProfileDtos;
using E_Commerce.Model;
using E_Commerce.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace E_Commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileService profileService;

        public ProfileController(IProfileService profileService)
        {
            this.profileService = profileService;
        }

        [HttpGet]
        public async Task<IActionResult> GetProfile()
        {
            string UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var profile = await profileService.GetProfile(UserId);

            if (profile is null)
                return NotFound(ApiResponse<object>.Failure("Profile Not Found"));

            return Ok(ApiResponse<GetProfileDto>.Success(profile));
        }

        [HttpPatch]
        public async Task<IActionResult> UpdateProfile(UpdateProfileDto dto)
        {
            string UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var Updatedprofile = await profileService.UpdateProfile(UserId, dto);

            if (Updatedprofile is null)
                return NotFound(ApiResponse<object>.Failure("Profile Not Found"));

            return Ok(ApiResponse<UpdateProfileDto>.Success(Updatedprofile));
        }

        [HttpGet("Profiles")]
        public async Task<IActionResult> GetProfilesForAdmin()
        {

            var profiles = await profileService.GetProfiles();

            if (profiles is null)
                return NotFound(ApiResponse<object>.Failure("Profile Not Found"));

            return Ok(ApiResponse<List<GetProfileForAdminDto>>.Success(profiles));
        }
    }
}
