using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mixtape.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddArtistCover : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CoverKey",
                table: "Artists",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Artists_CoverKey",
                table: "Artists",
                column: "CoverKey");

            migrationBuilder.AddForeignKey(
                name: "FK_Artists_Covers_CoverKey",
                table: "Artists",
                column: "CoverKey",
                principalTable: "Covers",
                principalColumn: "Key");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Artists_Covers_CoverKey",
                table: "Artists");

            migrationBuilder.DropIndex(
                name: "IX_Artists_CoverKey",
                table: "Artists");

            migrationBuilder.DropColumn(
                name: "CoverKey",
                table: "Artists");
        }
    }
}
