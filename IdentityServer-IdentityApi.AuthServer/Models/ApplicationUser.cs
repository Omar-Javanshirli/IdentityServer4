using Microsoft.AspNetCore.Identity;

namespace IdentityServer_IdentityApi.AuthServer.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public string City { get; set; }
    }
}
