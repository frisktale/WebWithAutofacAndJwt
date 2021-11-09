using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WebWithAutofacAndJwt.Entity;
public class AppDbContext : IdentityDbContext<User, IdentityRole<long>, long>
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
    : base(options)
    {
    }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<User>()
           .Property(b => b.Id)
           .ValueGeneratedNever();
    }
}

