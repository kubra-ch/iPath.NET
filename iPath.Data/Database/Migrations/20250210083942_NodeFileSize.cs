using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace iPath.Data.Database.Migrations
{
    /// <inheritdoc />
    public partial class NodeFileSize : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Filesize",
                table: "NodeFile",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Originalname",
                table: "NodeFile",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Filesize",
                table: "NodeFile");

            migrationBuilder.DropColumn(
                name: "Originalname",
                table: "NodeFile");
        }
    }
}
