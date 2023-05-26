using IdentityServer.AuthServer.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer.AuthServer.Repository
{
    public class CustomUserRepository : ICustomUserRepository
    {
        private readonly CustomDbContext context;

        public CustomUserRepository(CustomDbContext context)
        {
            this.context = context;
        }

        public async Task<CustomUser> FindByEmail(string email)
        {
            return await this.context.CustomUsers.FirstOrDefaultAsync(x => x.Email == email);
        }

        public async Task<CustomUser> FindById(int id)
        {
            //FindAsync methodu bir basa primary keye gore axtarix edir.Cox suretdi cavab gelir
            return await context.CustomUsers.FindAsync(id);
        }

        public async Task<bool> Validate(string email, string password)
        {
            return await this.context.CustomUsers.AnyAsync(x=>x.Email== email && x.Password == password);
        }
    }
}
