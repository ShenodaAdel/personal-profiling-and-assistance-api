using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class implementQuestionChoice2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuestionChoice_Choices_choiceId",
                table: "QuestionChoice");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionChoice_Questions_questionId",
                table: "QuestionChoice");

            migrationBuilder.DropPrimaryKey(
                name: "PK_QuestionChoice",
                table: "QuestionChoice");

            migrationBuilder.RenameTable(
                name: "QuestionChoice",
                newName: "QuestionChoices");

            migrationBuilder.RenameIndex(
                name: "IX_QuestionChoice_questionId",
                table: "QuestionChoices",
                newName: "IX_QuestionChoices_questionId");

            migrationBuilder.RenameIndex(
                name: "IX_QuestionChoice_choiceId",
                table: "QuestionChoices",
                newName: "IX_QuestionChoices_choiceId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_QuestionChoices",
                table: "QuestionChoices",
                column: "Id");

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuestionChoices_Choices_choiceId",
                table: "QuestionChoices");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionChoices_Questions_questionId",
                table: "QuestionChoices");

            migrationBuilder.DropPrimaryKey(
                name: "PK_QuestionChoices",
                table: "QuestionChoices");

            migrationBuilder.RenameTable(
                name: "QuestionChoices",
                newName: "QuestionChoice");

            migrationBuilder.RenameIndex(
                name: "IX_QuestionChoices_questionId",
                table: "QuestionChoice",
                newName: "IX_QuestionChoice_questionId");

            migrationBuilder.RenameIndex(
                name: "IX_QuestionChoices_choiceId",
                table: "QuestionChoice",
                newName: "IX_QuestionChoice_choiceId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_QuestionChoice",
                table: "QuestionChoice",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionChoice_Choices_choiceId",
                table: "QuestionChoice",
                column: "choiceId",
                principalTable: "Choices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionChoice_Questions_questionId",
                table: "QuestionChoice",
                column: "questionId",
                principalTable: "Questions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
