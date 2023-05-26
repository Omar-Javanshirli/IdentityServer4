using IdentityServer.AuthServer.Repository;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentityServer.AuthServer.Services
{
    public class CustomProfileService : IProfileService
    {
        private readonly ICustomUserRepository customUserRepository;

        public CustomProfileService(ICustomUserRepository customUserRepository)
        {
            this.customUserRepository = customUserRepository;
        }

        //Userinfo Endpoint-e sorgu gonderdiyimiz anda bu method isleyecek
        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var subId = context.Subject.GetSubjectId();

            var user = await this.customUserRepository.FindById(int.Parse(subId));

            var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Email,user.Email),
                new Claim(ClaimTypes.Name,user.Username),
                new Claim("city",user.City),
            };

            if (user.Id == 1)
                claims.Add(new Claim("role", "admin"));
            else
                claims.Add(new Claim("role", "customer"));

            context.AddRequestedClaims(claims);
            
            //Eger biz bunu yazarsax Claim-ler bir basa Jwt-nin icine gomulur. ama duzgun yazilis deyildir. 
            //duzgun yazilis yuxarida ki kimidir.
            context.IssuedClaims= claims;
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            var userId = context.Subject.GetSubjectId();

            var user = await this.customUserRepository.FindById(int.Parse(userId));

            context.IsActive = user != null ? true : false;
        }
    }
}
