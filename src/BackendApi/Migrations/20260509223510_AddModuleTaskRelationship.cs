using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendApi.Migrations
{
    /// <inheritdoc />
    public partial class AddModuleTaskRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Tasks_ModuleId",
                table: "Tasks",
                column: "ModuleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Modules_ModuleId",
                table: "Tasks",
                column: "ModuleId",
                principalTable: "Modules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Modules_ModuleId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_ModuleId",
                table: "Tasks");
        }
    }
}
