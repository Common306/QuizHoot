using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace QuizHoot.Migrations
{
    public partial class QuestionMultipleChoice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "FinishAt",
                table: "TakeQuiz",
                type: "datetime",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime");

            migrationBuilder.AddColumn<bool>(
                name: "MultipleChoice",
                table: "Question",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MultipleChoice",
                table: "Question");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FinishAt",
                table: "TakeQuiz",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true);
        }
    }
}
