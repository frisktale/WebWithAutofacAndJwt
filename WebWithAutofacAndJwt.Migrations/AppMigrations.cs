using Microsoft.EntityFrameworkCore;
using WebWithAutofacAndJwt.Entity;

namespace WebWithAutofacAndJwt.Migrations
{
    public class AppMigrations : AppDbContext
    {
        public AppMigrations(DbContextOptions<AppDbContext> options) : base(options)
        {
            
        }
    }
}