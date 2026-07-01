using Microsoft.AspNetCore.Identity;

namespace E_Commerce.Model
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;

        public Cart Cart { get; set; } = null!;

        public List<Order> Orders { get; set; } = new List<Order>();

        public List<Comment> Comments { get; set; } = new List<Comment>();

        public List<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}
