using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace iPath.Data.Database.Migrations
{
    /// <inheritdoc />
    public partial class UserSysAdmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSysAdmin",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSysAdmin",
                table: "Users");
        }
    }
}
