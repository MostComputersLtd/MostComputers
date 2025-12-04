using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MOSTComputers.Services.Identity.Models;
using System.Linq.Expressions;
using System.Reflection;

namespace MOSTComputers.Services.Identity.DAL;
internal class PasswordsTableOnlyAuthenticationDBContext : DbContext
{
    public PasswordsTableOnlyAuthenticationDBContext(DbContextOptions<PasswordsTableOnlyAuthenticationDBContext> options)
        : base(options)
    {
    }

    public DbSet<PasswordsTableOnlyUser> Users { get; set; }
    public DbSet<PasswordsTableOnlyRole> UserRoles { get; set; }
    public DbSet<IdentityUserRole<string>> RolesForUser { get; set; }
    public DbSet<IdentityUserClaim<string>> UserClaims { get; set; }
    public DbSet<IdentityRoleClaim<string>> UserRoleClaims { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<PasswordsTableOnlyUser>().ToTable("Users");

        builder.Entity<PasswordsTableOnlyRole>().ToTable("UserRoles");

        builder.Entity<IdentityUserRole<string>>().ToTable("RolesForUser")
            .HasKey("UserId", "RoleId");

        builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims");

        builder.Entity<IdentityRoleClaim<string>>().ToTable("UserRoleClaims");
    }
}