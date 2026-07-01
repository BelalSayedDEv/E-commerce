using System.ComponentModel.DataAnnotations.Schema;

namespace E_Commerce.Model
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public string Token { get; set; } = string.Empty;
        public bool IsUsed { get; set; } = false;
        public DateTime ExpiresAt { get; set; }
        public bool IsRevoked { get; set; } = false;

        [ForeignKey("UserId")]
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;
    }
}
