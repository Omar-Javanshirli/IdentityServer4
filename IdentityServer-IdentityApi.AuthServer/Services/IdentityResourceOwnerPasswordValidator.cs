using IdentityModel;
using IdentityServer_IdentityApi.AuthServer.Models;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace IdentityServer.IdentityApi.AuthServer.Services
{
    public class IdentityResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        private readonly UserManager<ApplicationUser> userManager;

        public IdentityResourceOwnerPasswordValidator(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
        }

        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            var existUser = await this.userManager.FindByEmailAsync(context.UserName);
            if (existUser == null)
                return;

            var passwordCheck = await this.userManager.CheckPasswordAsync(existUser, context.Password);

            if (passwordCheck == false) return;

            context.Result = new GrantValidationResult(existUser.Id.ToString(), OidcConstants.AuthenticationMethods.Password);
        }
    }
}
