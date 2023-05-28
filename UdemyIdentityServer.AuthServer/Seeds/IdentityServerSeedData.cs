using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.EntityFrameworkCore.Internal;
using System.Linq;
using UdemyIdentityServer.AuthServer;

namespace IdentityServer.AuthServer.Seeds
{
    public static class IdentityServerSeedData
    {
        public static void Seed(ConfigurationDbContext context)
        {
            if (!context.Clients.Any())
            {
                foreach (var client in Config.GetClients())
                    context.Clients.Add(client.ToEntity());
            }

            if(!context.ApiResources.Any())
            {
                foreach (var item in Config.GetApiResources())
                    context.ApiResources.Add(item.ToEntity());
            }

            if (!context.ApiScopes.Any())
            {
                Config.GetApiScopes().ToList().ForEach(x =>
                {
                    context.ApiScopes.Add(x.ToEntity());
                });
            }

            if (!context.ApiScopes.Any())
            {
                Config.GetIdentityResources().ToList().ForEach(x =>
                {
                    context.IdentityResources.Add(x.ToEntity());
                });
            }

            context.SaveChanges();
        }
    }
}
