using Microsoft.EntityFrameworkCore;
using WebWithAutofacAndJwt.Entity;

namespace WebWithAutofacAndJwt.Migrations;
/// <summary>
/// 继承自Entity的DbContext，这样就可以在该项目中保存迁移文件
/// </summary>
public class AppMigrations : AppDbContext
{
    public AppMigrations(DbContextOptions<AppDbContext> options) : base(options)
    {

    }
}