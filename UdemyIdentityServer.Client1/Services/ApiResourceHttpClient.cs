using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Net.Http;
using System.Threading.Tasks;

namespace UdemyIdentityServer.Client1.Services
{
    public class ApiResourceHttpClient : IApiResourceHttpClient
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        private HttpClient _client;
        public ApiResourceHttpClient(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _client = new HttpClient();
        }

        public async Task<HttpClient> GetHttpClient()
        {
            var accessToken = await _httpContextAccessor.HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);

            //Api-ye sorgu gondezden evven sorgu gondereceyim request-in body hissesinde Authorozation Key ve Value-nu gondermey lazimdir.
            //bu code hemen tokeni elaqeli requestin header hissesine yazir. Tokeni Authorizatio  header de set edirik.
            _client.SetBearerToken(accessToken);

            return _client;
        }
    }
}