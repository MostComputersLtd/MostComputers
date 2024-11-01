using Microsoft.EntityFrameworkCore;
using MOSTComputers.Services.Identity.Models;

namespace MOSTComputers.Services.Identity.DAL;
internal class PasswordsTableOnlyAuthenticationDBContext : DbContext
{
    public PasswordsTableOnlyAuthenticationDBContext(DbContextOptions<PasswordsTableOnlyAuthenticationDBContext> options)
        : base(options)
    {
    }

    public DbSet<PasswordsTableOnlyUser> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
    }
}