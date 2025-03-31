using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class updatePropertyUserTest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserTests_AspNetUsers_UserId1",
                table: "UserTests");

            migrationBuilder.DropIndex(
                name: "IX_UserTests_UserId1",
                table: "UserTests");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "UserTests");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "UserTests",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserTests_UserId",
                table: "UserTests",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserTests_AspNetUsers_UserId",
                table: "UserTests",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserTests_AspNetUsers_UserId",
                table: "UserTests");

            migrationBuilder.DropIndex(
                name: "IX_UserTests_UserId",
                table: "UserTests");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "UserTests",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId1",
                table: "UserTests",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserTests_UserId1",
                table: "UserTests",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_UserTests_AspNetUsers_UserId1",
                table: "UserTests",
                column: "UserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
