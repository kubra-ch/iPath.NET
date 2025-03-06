using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace iPath.Migrations.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class DleteFlagsIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Users_DeletedOn",
                table: "Users",
                column: "DeletedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Nodes_DeletedOn",
                table: "Nodes",
                column: "DeletedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Groups_DeletedOn",
                table: "Groups",
                column: "DeletedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Communities_DeletedOn",
                table: "Communities",
                column: "DeletedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Annotations_DeletedOn",
                table: "Annotations",
                column: "DeletedOn");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_DeletedOn",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Nodes_DeletedOn",
                table: "Nodes");

            migrationBuilder.DropIndex(
                name: "IX_Groups_DeletedOn",
                table: "Groups");

            migrationBuilder.DropIndex(
                name: "IX_Communities_DeletedOn",
                table: "Communities");

            migrationBuilder.DropIndex(
                name: "IX_Annotations_DeletedOn",
                table: "Annotations");
        }
    }
}
