using IdentityModel.Client;
using IdentityServer.Client1.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace UdemyIdentityServer.Client1.Services
{
    public class ApiResourceHttpClient : IApiResourceHttpClient
    {
        //HttpContext-de yalniz contorller daxilende catmax olduguna gore burda IHttpContextAccessor Interfacinden istifade edirik
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly HttpClient _client;
        private readonly IConfiguration configuration;

        public ApiResourceHttpClient(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _httpContextAccessor = httpContextAccessor;
            _client = new HttpClient();
            this.configuration = configuration;
        }

        public async Task<HttpClient> GetHttpClient()
        {
            var accessToken = await _httpContextAccessor.HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);

            //Api-ye sorgu gondezden evven sorgu gondereceyim request-in body hissesinde Authorozation Key ve Value-nu gondermey lazimdir.
            //bu code hemen tokeni elaqeli requestin header hissesine yazir. Tokeni Authorizatio  header de set edirik. 
            _client.SetBearerToken(accessToken);

            return _client;
        }

        public async Task<List<string>> SaveUserViewModel(UserSaveViewModel request)
        {
            var disco = await _client.GetDiscoveryDocumentAsync(this.configuration["AuthServerUrl"]);
            if (disco.IsError)
            {
                //loglama yap
            }

            var clientCredentialsTokenRequest = new ClientCredentialsTokenRequest();
            clientCredentialsTokenRequest.ClientId = this.configuration["ClientResourceOwner:ClientId"];
            clientCredentialsTokenRequest.ClientSecret = this.configuration["ClientResourceOwner:ClientSecret"];
            clientCredentialsTokenRequest.Address = disco.TokenEndpoint;

            var token = await _client.RequestClientCredentialsTokenAsync(clientCredentialsTokenRequest);

            if (token.IsError)
            {
                //loglama yap
            }

            //StringContect HttpContente qarsilig gelir
            var stringContent = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

            _client.SetBearerToken(token.AccessToken);

            var response = await _client.PostAsync("https://localhost:5001/api/user/signup", stringContent);

            if(!response.IsSuccessStatusCode)
            {
                var errors= JsonConvert.DeserializeObject<List<string>>( await response.Content.ReadAsStringAsync());
                return errors;
            }
            return null;
        }
    }
}