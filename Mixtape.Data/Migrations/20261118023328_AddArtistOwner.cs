using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mixtape.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddArtistOwner : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UserKey",
                table: "Artists",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Artists_UserKey",
                table: "Artists",
                column: "UserKey");

            migrationBuilder.AddForeignKey(
                name: "FK_Artists_Users_UserKey",
                table: "Artists",
                column: "UserKey",
                principalTable: "Users",
                principalColumn: "Key");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Artists_Users_UserKey",
                table: "Artists");

            migrationBuilder.DropIndex(
                name: "IX_Artists_UserKey",
                table: "Artists");

            migrationBuilder.DropColumn(
                name: "UserKey",
                table: "Artists");
        }
    }
}
