using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Music_Library_Management_Application.Migrations
{
    /// <inheritdoc />
    public partial class ExtendedIdentityV1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SongPlaylists_Playlists_PlaylistId",
                table: "SongPlaylists");

            migrationBuilder.DropForeignKey(
                name: "FK_SongPlaylists_Songs_SongId",
                table: "SongPlaylists");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SongPlaylists",
                table: "SongPlaylists");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Songs",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "SongPlaylists",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Playlists",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "AspNetUsers",
                type: "nvarchar(13)",
                maxLength: 13,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SongPlaylists",
                table: "SongPlaylists",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Songs_UserId",
                table: "Songs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SongPlaylists_SongId",
                table: "SongPlaylists",
                column: "SongId");

            migrationBuilder.CreateIndex(
                name: "IX_Playlists_UserId",
                table: "Playlists",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Playlists_AspNetUsers_UserId",
                table: "Playlists",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SongPlaylists_Playlists_PlaylistId",
                table: "SongPlaylists",
                column: "PlaylistId",
                principalTable: "Playlists",
                principalColumn: "PlaylistId");

            migrationBuilder.AddForeignKey(
                name: "FK_SongPlaylists_Songs_SongId",
                table: "SongPlaylists",
                column: "SongId",
                principalTable: "Songs",
                principalColumn: "SongId");

            migrationBuilder.AddForeignKey(
                name: "FK_Songs_AspNetUsers_UserId",
                table: "Songs",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Playlists_AspNetUsers_UserId",
                table: "Playlists");

            migrationBuilder.DropForeignKey(
                name: "FK_SongPlaylists_Playlists_PlaylistId",
                table: "SongPlaylists");

            migrationBuilder.DropForeignKey(
                name: "FK_SongPlaylists_Songs_SongId",
                table: "SongPlaylists");

            migrationBuilder.DropForeignKey(
                name: "FK_Songs_AspNetUsers_UserId",
                table: "Songs");

            migrationBuilder.DropIndex(
                name: "IX_Songs_UserId",
                table: "Songs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SongPlaylists",
                table: "SongPlaylists");

            migrationBuilder.DropIndex(
                name: "IX_SongPlaylists_SongId",
                table: "SongPlaylists");

            migrationBuilder.DropIndex(
                name: "IX_Playlists_UserId",
                table: "Playlists");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Songs");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "SongPlaylists");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Playlists");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "AspNetUsers");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SongPlaylists",
                table: "SongPlaylists",
                columns: new[] { "SongId", "PlaylistId" });

            migrationBuilder.AddForeignKey(
                name: "FK_SongPlaylists_Playlists_PlaylistId",
                table: "SongPlaylists",
                column: "PlaylistId",
                principalTable: "Playlists",
                principalColumn: "PlaylistId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SongPlaylists_Songs_SongId",
                table: "SongPlaylists",
                column: "SongId",
                principalTable: "Songs",
                principalColumn: "SongId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
