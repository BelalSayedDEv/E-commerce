using E_Commerce.DTos.ProfileDtos;

namespace E_Commerce.Services
{
    public interface IProfileService
    {
        public Task<GetProfileDto?> GetProfile(string UserId);
        public Task<UpdateProfileDto?> UpdateProfile(string UserId, UpdateProfileDto dto);

        public Task<List<GetProfileForAdminDto>> GetProfiles();
    }
}
