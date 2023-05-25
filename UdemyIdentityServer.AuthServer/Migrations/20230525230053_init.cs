using Microsoft.EntityFrameworkCore.Migrations;

namespace IdentityServer.AuthServer.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CustomUsers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    Password = table.Column<string>(nullable: true),
                    City = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomUsers", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "CustomUsers",
                columns: new[] { "Id", "City", "Email", "Password", "Username" },
                values: new object[] { 1, "Baki", "omer@gamil.com", "password", "omar_javanshirli" });

            migrationBuilder.InsertData(
                table: "CustomUsers",
                columns: new[] { "Id", "City", "Email", "Password", "Username" },
                values: new object[] { 2, "Lacin", "amin@gamil.com", "password", "amin" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomUsers");
        }
    }
}
