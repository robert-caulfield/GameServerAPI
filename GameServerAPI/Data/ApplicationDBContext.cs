using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using GameServerAPI.Models;
using Microsoft.AspNetCore.Identity;
using System.Reflection.Emit;

namespace GameServerAPI.Data
{
    /// <summary>
    /// Represents database context for the API
    /// </summary>
    public class ApplicationDBContext : IdentityDbContext<ApplicationUser>
    {
        /// <summary>
        /// Initializes instance of ApplicationDBContext class.
        /// </summary>
        /// <param name="options">Instance of DBContext options to be used by the DbContext.</param>
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
        {
            
        }
        /// <summary>DBSet of application users.</summary>
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        /// <summary>DBSet of Game Servers.</summary>
        public DbSet<GameServer> GameServers { get; set; }



        /// <summary>
        /// Configure schema need for the identity framework and seeds initial data including
        /// 'Player' and 'Admin' roles and a base admin user.
        /// </summary>
        /// <param name="builder"></param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure the one-to-many relationship between ApplicationUser and GameServer
            builder.Entity<GameServer>()
            .HasOne(g => g.Creator)
            .WithMany() // No collection on the User side
            .HasForeignKey(g => g.CreatorUserId);

            // Seed roles
            builder.Entity<IdentityRole>().HasData(new IdentityRole { Id = "1", Name = "Admin", NormalizedName = "ADMIN" },
                                                   new IdentityRole { Id = "2", Name = "Server", NormalizedName = "SERVER" },
                                                   new IdentityRole { Id = "3", Name = "Player", NormalizedName = "PLAYER" });

            // Hasher to hash password before seeding
            PasswordHasher<ApplicationUser> hasher = new PasswordHasher<ApplicationUser>();

            // Seed admin user
            ApplicationUser admin = new ApplicationUser
            {
                Id = "1",
                UserName = "admin",
                NormalizedUserName = "ADMIN",
                Email = "admin@example.com",
                NormalizedEmail = "ADMIN@EXAMPLE.COM",
                PasswordHash = hasher.HashPassword(null, "Admin123$"),
                SecurityStamp = Guid.NewGuid().ToString()
            };

            // Seed server user
            ApplicationUser server = new ApplicationUser
            {
                Id = "2",
                UserName = "server",
                NormalizedUserName = "SERVER",
                Email = "server@example.com",
                NormalizedEmail = "SERVER@EXAMPLE.COM",
                PasswordHash = hasher.HashPassword(null, "Server123$"),
                SecurityStamp = Guid.NewGuid().ToString()
            };

            // Seed admin and server users
            builder.Entity<ApplicationUser>().HasData(admin, server);

            // Grant roles to new users
            builder.Entity<IdentityUserRole<string>>().HasData(
            new IdentityUserRole<string>
            { // Admin
                UserId = "1", // Admin Id
                RoleId = "1", // Role Id of Admin
            },
            new IdentityUserRole<string>
            { // Server
                UserId = "2", // Server Id
                RoleId = "2", // Role Id of Server
            });

        }
    }
}
