using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Music_Library_Management_Application.Migrations
{
    /// <inheritdoc />
    public partial class MixDbAndMixSongDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Mixes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MixFile = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    LimiterThreshold = table.Column<double>(type: "float", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mixes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Mixes_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MixSongs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MixDbId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartTime = table.Column<double>(type: "float", nullable: false),
                    EndTime = table.Column<double>(type: "float", nullable: false),
                    StartPosition = table.Column<double>(type: "float", nullable: false),
                    FadeInDuration = table.Column<double>(type: "float", nullable: false),
                    FadeOutDuration = table.Column<double>(type: "float", nullable: false),
                    Volume = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MixSongs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MixSongs_Mixes_MixDbId",
                        column: x => x.MixDbId,
                        principalTable: "Mixes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Mixes_UserId",
                table: "Mixes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_MixSongs_MixDbId",
                table: "MixSongs",
                column: "MixDbId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MixSongs");

            migrationBuilder.DropTable(
                name: "Mixes");
        }
    }
}
