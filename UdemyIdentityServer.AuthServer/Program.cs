using IdentityServer.AuthServer.Seeds;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace UdemyIdentityServer.AuthServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //Startup file-daki servicederi bu kod il elde etmek olur.
            var host = CreateHostBuilder(args).Build();

            //Burda bildirik ki men DI Container-de bir dene service catacam.Isim qutardigi zaman Bunu dispose et Memoride tutma.
            //bu na gore Using blokunnan istifade edirik. 
            using (var serviceScope = host.Services.CreateScope())
            {
                var services = serviceScope.ServiceProvider;
                var context = services.GetRequiredService<ConfigurationDbContext>();

                IdentityServerSeedData.Seed(context);
            }

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
