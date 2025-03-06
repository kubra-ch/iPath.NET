using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace iPath.Migrations.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class DleteFlags : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedOn",
                table: "Users",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedOn",
                table: "Nodes",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedOn",
                table: "Groups",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedOn",
                table: "Communities",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedOn",
                table: "Annotations",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeletedOn",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "DeletedOn",
                table: "Nodes");

            migrationBuilder.DropColumn(
                name: "DeletedOn",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "DeletedOn",
                table: "Communities");

            migrationBuilder.DropColumn(
                name: "DeletedOn",
                table: "Annotations");
        }
    }
}
