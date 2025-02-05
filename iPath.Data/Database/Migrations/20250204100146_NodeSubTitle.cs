using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace iPath.Data.Database.Migrations
{
    /// <inheritdoc />
    public partial class NodeSubTitle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SubType",
                table: "Nodes");

            migrationBuilder.AddColumn<string>(
                name: "SubTitle",
                table: "Nodes",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SubTitle",
                table: "Nodes");

            migrationBuilder.AddColumn<string>(
                name: "SubType",
                table: "Nodes",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }
    }
}
