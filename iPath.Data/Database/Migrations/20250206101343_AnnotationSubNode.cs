using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace iPath.Data.Database.Migrations
{
    /// <inheritdoc />
    public partial class AnnotationSubNode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Text",
                table: "NodeAnnotations",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "OwnerId",
                table: "NodeAnnotations",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SubNodeId",
                table: "NodeAnnotations",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_NodeAnnotations_CreatedOn",
                table: "NodeAnnotations",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_NodeAnnotations_Visibility",
                table: "NodeAnnotations",
                column: "Visibility");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_NodeAnnotations_CreatedOn",
                table: "NodeAnnotations");

            migrationBuilder.DropIndex(
                name: "IX_NodeAnnotations_Visibility",
                table: "NodeAnnotations");

            migrationBuilder.DropColumn(
                name: "SubNodeId",
                table: "NodeAnnotations");

            migrationBuilder.AlterColumn<string>(
                name: "Text",
                table: "NodeAnnotations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "OwnerId",
                table: "NodeAnnotations",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
