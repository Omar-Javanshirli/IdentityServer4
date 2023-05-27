using IdentityModel;
using IdentityServer.AuthServer.Repository;
using IdentityServer4.Validation;
using System.Threading.Tasks;

namespace IdentityServer.AuthServer.Services
{
    public class ResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        private readonly ICustomUserRepository customUserRepository;

        public ResourceOwnerPasswordValidator(ICustomUserRepository customUserRepository)
        {
            this.customUserRepository = customUserRepository;
        }

        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            var isUser= await this.customUserRepository.Validate(context.UserName,context.Password);

            if(isUser)
            {
                var user = await this.customUserRepository.FindByEmail(context.UserName);

                //Buradaki Password Resource Owner Credentials axisini bildirir. yani User bu axis ile login ve ya sigin olacag.
                //Resource Owner Credentials axsinin qisaldilmasi Password olarag yazilir ve ya pwd olarag yazilir.
                context.Result = new GrantValidationResult(user.Id.ToString(), OidcConstants.AuthenticationMethods.Password);
            }
        }
    }
}
