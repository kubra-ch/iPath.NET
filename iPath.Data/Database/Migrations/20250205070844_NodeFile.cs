using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace iPath.Data.Database.Migrations
{
    /// <inheritdoc />
    public partial class NodeFile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Filename",
                table: "Nodes");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Nodes",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "NodeFile",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NodeId = table.Column<int>(type: "int", nullable: false),
                    Filename = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    MimeType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ThumbData = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsImage = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NodeFile", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NodeFile_Nodes_NodeId",
                        column: x => x.NodeId,
                        principalTable: "Nodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NodeFile_NodeId",
                table: "NodeFile",
                column: "NodeId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NodeFile");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Nodes",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "Filename",
                table: "Nodes",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);
        }
    }
}
