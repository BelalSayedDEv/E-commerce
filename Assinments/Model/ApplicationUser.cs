using Microsoft.AspNetCore.Identity;

namespace Assinments.Model
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
        public string Address { get; set; }

        public Cart Cart { get; set; }

        public List<Order> Orders { get; set; }
    }
}
