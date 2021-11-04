using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebWithAutofacAndJwt.Migrations.Migrations
{
    public partial class NewMigrationv101 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "AspNetUsers",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldDefaultValue: 220587913658437L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "AspNetUsers",
                type: "bigint",
                nullable: false,
                defaultValue: 220587913658437L,
                oldClrType: typeof(long),
                oldType: "bigint");
        }
    }
}
