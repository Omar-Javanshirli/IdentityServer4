using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static IdentityServer4.IdentityServerConstants;

namespace IdentityServer.IdentityApi.AuthServer.Controllers
{
  
    [Route("api/[controller]/[action]")]
    [Authorize(LocalApi.PolicyName)]
    [ApiController]
    public class UserController : ControllerBase
    {
        [HttpPost]
        public IActionResult SignUp()
        {
            return Ok("signup calisti");
        }
    }
}
