using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MOSTComputers.Services.Identity.DAL;

public sealed class DefaultAuthenticationDBContext : IdentityDbContext<IdentityUser>
{
    public DefaultAuthenticationDBContext()
    {
        
    }

    public DefaultAuthenticationDBContext(DbContextOptions options)
        : base(options)
    {
    }

    // For migration purposes (temporarely)

    //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //{
    //    optionsBuilder.UseSqlServer("Data Source=(local);Initial Catalog=MOSTComputers.Services.Authentication;Integrated Security=True;Encrypt=False;");

    //    base.OnConfiguring(optionsBuilder);
    //}

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
    }
}