using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace iPath.Data.Database.Migrations
{
    /// <inheritdoc />
    public partial class NodeTypeEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Nodes_NodeType_NodeTypeId",
                table: "Nodes");

            migrationBuilder.DropTable(
                name: "NodeType");

            migrationBuilder.DropIndex(
                name: "IX_Nodes_NodeTypeId",
                table: "Nodes");

            migrationBuilder.RenameColumn(
                name: "NodeTypeId",
                table: "Nodes",
                newName: "NodeType");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NodeType",
                table: "Nodes",
                newName: "NodeTypeId");

            migrationBuilder.CreateTable(
                name: "NodeType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AcceptChildNodes = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NodeType", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Nodes_NodeTypeId",
                table: "Nodes",
                column: "NodeTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Nodes_NodeType_NodeTypeId",
                table: "Nodes",
                column: "NodeTypeId",
                principalTable: "NodeType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
