using E_Commerce.DTos.ProfileDtos;
using E_Commerce.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Services
{
    public class ProfileService : IProfileService
    {
        private readonly UserManager<ApplicationUser> userManager;

        public ProfileService(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
        }

        public async Task<GetProfileDto?> GetProfile(string UserId)
        {
            var User = await userManager.FindByIdAsync(UserId);

            if (User == null)
                return null;

            return new GetProfileDto
            {
                FullName = User.FullName,
                Email = User.Email!,
                Address = User.Address,

            };

        }

        public async Task<UpdateProfileDto?> UpdateProfile(string UserId, UpdateProfileDto dto)
        {
            var User = await userManager.FindByIdAsync(UserId);

            if (User == null)
                return null;

            if (dto.FullName is not null)
                User.FullName = dto.FullName;

            if (dto.Email is not null)
                User.Email = dto.Email;

            if (dto.Address is not null)
                User.Address = dto.Address;


            await userManager.UpdateAsync(User);

            dto.Address = User.Address;
            dto.Email = User.Email!;
            dto.FullName = User.FullName;

            return dto;
        }

        public async Task<List<GetProfileForAdminDto>> GetProfiles()
        {
            var Users = await userManager.Users.ToListAsync();
            List<GetProfileForAdminDto> Profiles = new List<GetProfileForAdminDto>();

            foreach (var user in Users)
            {
                GetProfileForAdminDto profileDto = new GetProfileForAdminDto();

                profileDto.UserName = user.UserName!;
                profileDto.FullName = user.FullName;
                profileDto.Email = user.Email!;
                profileDto.Address = user.Address;

                Profiles.Add(profileDto);
            }

            return Profiles;
        }
    }
}
