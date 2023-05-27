using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace IdentityServer.AuthServer.Midlwares
{
    public class CspMiddleware
    {
        private readonly RequestDelegate _next;

        public CspMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            // Set the Content-Security-Policy header here
            context.Response.Headers.Add("Content-Security-Policy", "default-src 'self'; connect-src 'self' wss://localhost:44366;");

            // Call the next middleware in the pipeline
            await _next(context);
        }
    }
}
