using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace connectify.Data.Migrations
{
    /// <inheritdoc />
    public partial class FriendsDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Friends_AspNetUsers_FriendId",
                table: "Friends");

            migrationBuilder.RenameColumn(
                name: "FriendId",
                table: "Friends",
                newName: "UserFriendId");

            migrationBuilder.RenameIndex(
                name: "IX_Friends_FriendId",
                table: "Friends",
                newName: "IX_Friends_UserFriendId");

            migrationBuilder.AddForeignKey(
                name: "FK_Friends_AspNetUsers_UserFriendId",
                table: "Friends",
                column: "UserFriendId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Friends_AspNetUsers_UserFriendId",
                table: "Friends");

            migrationBuilder.RenameColumn(
                name: "UserFriendId",
                table: "Friends",
                newName: "FriendId");

            migrationBuilder.RenameIndex(
                name: "IX_Friends_UserFriendId",
                table: "Friends",
                newName: "IX_Friends_FriendId");

            migrationBuilder.AddForeignKey(
                name: "FK_Friends_AspNetUsers_FriendId",
                table: "Friends",
                column: "FriendId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
