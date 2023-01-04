using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace connectify.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddNewInfoUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Visibility",
                table: "AspNetUsers",
                type: "bit",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Visibility",
                table: "AspNetUsers");
        }
    }
}
