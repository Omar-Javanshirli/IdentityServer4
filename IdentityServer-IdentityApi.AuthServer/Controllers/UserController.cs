using IdentityServer.IdentityApi.AuthServer.Models;
using IdentityServer_IdentityApi.AuthServer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using static IdentityServer4.IdentityServerConstants;

namespace IdentityServer.IdentityApi.AuthServer.Controllers
{

    [Route("api/[controller]/[action]")]
    [Authorize(LocalApi.PolicyName)]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;

        public UserController(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(UserSaveViewModel request)
        {
            ApplicationUser applicationUser = new ApplicationUser();

            applicationUser.UserName = request.Username;
            applicationUser.Email = request.Email;
            applicationUser.City = request.City;

            var result = await this.userManager.CreateAsync(applicationUser, request.Password);

            if(!result.Succeeded)
                return BadRequest(result.Errors.Select(x=>x.Description));

            return Ok("signup calisti");
        }
    }
}
