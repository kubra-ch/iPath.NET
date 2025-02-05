using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace iPath.Data.Database.Migrations
{
    /// <inheritdoc />
    public partial class Annotation_Node : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NodeAnnotations_Nodes_ParentObjectId",
                table: "NodeAnnotations");

            migrationBuilder.RenameColumn(
                name: "ParentObjectId",
                table: "NodeAnnotations",
                newName: "NodeId");

            migrationBuilder.RenameIndex(
                name: "IX_NodeAnnotations_ParentObjectId",
                table: "NodeAnnotations",
                newName: "IX_NodeAnnotations_NodeId");

            migrationBuilder.AddForeignKey(
                name: "FK_NodeAnnotations_Nodes_NodeId",
                table: "NodeAnnotations",
                column: "NodeId",
                principalTable: "Nodes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NodeAnnotations_Nodes_NodeId",
                table: "NodeAnnotations");

            migrationBuilder.RenameColumn(
                name: "NodeId",
                table: "NodeAnnotations",
                newName: "ParentObjectId");

            migrationBuilder.RenameIndex(
                name: "IX_NodeAnnotations_NodeId",
                table: "NodeAnnotations",
                newName: "IX_NodeAnnotations_ParentObjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_NodeAnnotations_Nodes_ParentObjectId",
                table: "NodeAnnotations",
                column: "ParentObjectId",
                principalTable: "Nodes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
