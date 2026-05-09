using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendApi.Migrations
{
    /// <inheritdoc />
    public partial class CreateUserTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "app_user",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", 1),
                    name = table.Column<string>(maxLength: 100, nullable: false),
                    email = table.Column<string>(maxLength: 255, nullable: false),
                    password_hash = table.Column<string>(maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_app_user", x => x.id);
                    table.UniqueConstraint("AK_app_user_email", x => x.email);
                });
            
            string hashedAdminPassword = BCrypt.Net.BCrypt.HashPassword("admin");

            migrationBuilder.InsertData(
                table: "app_user",
                columns: new[] { "name", "email", "password_hash" },
                values: new object[] { "admin", "admin@admin.admin", hashedAdminPassword });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "app_user");
        }
    }
}
