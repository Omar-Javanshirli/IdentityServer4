using Microsoft.EntityFrameworkCore;

namespace IdentityServer.AuthServer.Models
{
    public class CustomDbContext : DbContext
    {
        //Birden cox DbContext oldugu zaman Asagida gosderildiyi kimi Generic olarag Bildurmeliyik 
        public CustomDbContext(DbContextOptions<CustomDbContext> opts)
            : base(opts)
        {

        }

        public DbSet<CustomUser> CustomUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CustomUser>().HasData(
            new CustomUser()
            {
                Id = 1,
                Email = "omer@gamil.com",
                Password = "password",
                Username = "omar_javanshirli",
                City = "Baki"
            },
            new CustomUser()
            {
                Id = 2,
                Email = "amin@gamil.com",
                Password = "password",
                Username = "amin",
                City = "Lacin"
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
