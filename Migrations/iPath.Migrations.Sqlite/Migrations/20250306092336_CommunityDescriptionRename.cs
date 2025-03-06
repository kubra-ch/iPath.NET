using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace iPath.Migrations.Sqlite.Migrations
{
    /// <inheritdoc />
    public partial class CommunityDescriptionRename : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Descritption",
                table: "Communities",
                newName: "Description");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Communities",
                newName: "Descritption");
        }
    }
}
