using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace iPath.Data.Database.Migrations
{
    /// <inheritdoc />
    public partial class NodeFileImageSize : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ImageHeight",
                table: "NodeFile",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ImageWidth",
                table: "NodeFile",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageHeight",
                table: "NodeFile");

            migrationBuilder.DropColumn(
                name: "ImageWidth",
                table: "NodeFile");
        }
    }
}
