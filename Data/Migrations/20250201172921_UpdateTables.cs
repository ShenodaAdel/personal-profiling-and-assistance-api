using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuestionChoices_Choices_choiceId",
                table: "QuestionChoices");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionChoices_Questions_questionId",
                table: "QuestionChoices");

            migrationBuilder.DropForeignKey(
                name: "FK_Questions_Tests_testId",
                table: "Questions");

            migrationBuilder.DropForeignKey(
                name: "FK_UserTests_Tests_testId",
                table: "UserTests");

            migrationBuilder.DropForeignKey(
                name: "FK_UserTests_Users_userId",
                table: "UserTests");

            migrationBuilder.RenameColumn(
                name: "userId",
                table: "UserTests",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "testId",
                table: "UserTests",
                newName: "TestId");

            migrationBuilder.RenameIndex(
                name: "IX_UserTests_userId",
                table: "UserTests",
                newName: "IX_UserTests_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserTests_testId",
                table: "UserTests",
                newName: "IX_UserTests_TestId");

            migrationBuilder.RenameColumn(
                name: "testId",
                table: "Questions",
                newName: "TestId");

            migrationBuilder.RenameIndex(
                name: "IX_Questions_testId",
                table: "Questions",
                newName: "IX_Questions_TestId");

            migrationBuilder.RenameColumn(
                name: "questionId",
                table: "QuestionChoices",
                newName: "QuestionId");

            migrationBuilder.RenameColumn(
                name: "choiceId",
                table: "QuestionChoices",
                newName: "ChoiceId");

            migrationBuilder.RenameIndex(
                name: "IX_QuestionChoices_questionId",
                table: "QuestionChoices",
                newName: "IX_QuestionChoices_QuestionId");

            migrationBuilder.RenameIndex(
                name: "IX_QuestionChoices_choiceId",
                table: "QuestionChoices",
                newName: "IX_QuestionChoices_ChoiceId");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "UserTests",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "TestId",
                table: "UserTests",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "TestId",
                table: "Questions",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "QuestionId",
                table: "QuestionChoices",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "ChoiceId",
                table: "QuestionChoices",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionChoices_Choices_ChoiceId",
                table: "QuestionChoices",
                column: "ChoiceId",
                principalTable: "Choices",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionChoices_Questions_QuestionId",
                table: "QuestionChoices",
                column: "QuestionId",
                principalTable: "Questions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_Tests_TestId",
                table: "Questions",
                column: "TestId",
                principalTable: "Tests",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserTests_Tests_TestId",
                table: "UserTests",
                column: "TestId",
                principalTable: "Tests",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserTests_Users_UserId",
                table: "UserTests",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuestionChoices_Choices_ChoiceId",
                table: "QuestionChoices");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionChoices_Questions_QuestionId",
                table: "QuestionChoices");

            migrationBuilder.DropForeignKey(
                name: "FK_Questions_Tests_TestId",
                table: "Questions");

            migrationBuilder.DropForeignKey(
                name: "FK_UserTests_Tests_TestId",
                table: "UserTests");

            migrationBuilder.DropForeignKey(
                name: "FK_UserTests_Users_UserId",
                table: "UserTests");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "UserTests",
                newName: "userId");

            migrationBuilder.RenameColumn(
                name: "TestId",
                table: "UserTests",
                newName: "testId");

            migrationBuilder.RenameIndex(
                name: "IX_UserTests_UserId",
                table: "UserTests",
                newName: "IX_UserTests_userId");

            migrationBuilder.RenameIndex(
                name: "IX_UserTests_TestId",
                table: "UserTests",
                newName: "IX_UserTests_testId");

            migrationBuilder.RenameColumn(
                name: "TestId",
                table: "Questions",
                newName: "testId");

            migrationBuilder.RenameIndex(
                name: "IX_Questions_TestId",
                table: "Questions",
                newName: "IX_Questions_testId");

            migrationBuilder.RenameColumn(
                name: "QuestionId",
                table: "QuestionChoices",
                newName: "questionId");

            migrationBuilder.RenameColumn(
                name: "ChoiceId",
                table: "QuestionChoices",
                newName: "choiceId");

            migrationBuilder.RenameIndex(
                name: "IX_QuestionChoices_QuestionId",
                table: "QuestionChoices",
                newName: "IX_QuestionChoices_questionId");

            migrationBuilder.RenameIndex(
                name: "IX_QuestionChoices_ChoiceId",
                table: "QuestionChoices",
                newName: "IX_QuestionChoices_choiceId");

            migrationBuilder.AlterColumn<int>(
                name: "userId",
                table: "UserTests",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "testId",
                table: "UserTests",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "testId",
                table: "Questions",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "questionId",
                table: "QuestionChoices",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "choiceId",
                table: "QuestionChoices",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionChoices_Choices_choiceId",
                table: "QuestionChoices",
                column: "choiceId",
                principalTable: "Choices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionChoices_Questions_questionId",
                table: "QuestionChoices",
                column: "questionId",
                principalTable: "Questions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_Tests_testId",
                table: "Questions",
                column: "testId",
                principalTable: "Tests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserTests_Tests_testId",
                table: "UserTests",
                column: "testId",
                principalTable: "Tests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserTests_Users_userId",
                table: "UserTests",
                column: "userId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
