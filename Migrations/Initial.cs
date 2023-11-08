#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace EntityFrameworkCoreMultiTenancy.Migrations;

/// <inheritdoc />
public partial class Initial : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            "Members",
            table => new
            {
                MemberId = table.Column<int>("INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                Name = table.Column<string>("TEXT", nullable: false),
                Address = table.Column<string>("TEXT", nullable: false),
                Tenant = table.Column<string>("TEXT", nullable: false)
            },
            constraints: table => { table.PrimaryKey("PK_Members", m => m.MemberId); });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            "Members");
    }
}