using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace iPath.Data.Database.Migrations
{
    /// <inheritdoc />
    public partial class AnnotationReply : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CreateOn",
                table: "Nodes",
                newName: "CreatedOn");

            migrationBuilder.RenameIndex(
                name: "IX_Nodes_CreateOn",
                table: "Nodes",
                newName: "IX_Nodes_CreatedOn");

            migrationBuilder.AddColumn<int>(
                name: "ReplyToAnnotationId",
                table: "NodeAnnotations",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_NodeAnnotations_ReplyToAnnotationId",
                table: "NodeAnnotations",
                column: "ReplyToAnnotationId");

            migrationBuilder.AddForeignKey(
                name: "FK_NodeAnnotations_NodeAnnotations_ReplyToAnnotationId",
                table: "NodeAnnotations",
                column: "ReplyToAnnotationId",
                principalTable: "NodeAnnotations",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NodeAnnotations_NodeAnnotations_ReplyToAnnotationId",
                table: "NodeAnnotations");

            migrationBuilder.DropIndex(
                name: "IX_NodeAnnotations_ReplyToAnnotationId",
                table: "NodeAnnotations");

            migrationBuilder.DropColumn(
                name: "ReplyToAnnotationId",
                table: "NodeAnnotations");

            migrationBuilder.RenameColumn(
                name: "CreatedOn",
                table: "Nodes",
                newName: "CreateOn");

            migrationBuilder.RenameIndex(
                name: "IX_Nodes_CreatedOn",
                table: "Nodes",
                newName: "IX_Nodes_CreateOn");
        }
    }
}
