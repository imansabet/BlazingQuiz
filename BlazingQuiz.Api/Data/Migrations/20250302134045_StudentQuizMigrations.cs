using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazingQuiz.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class StudentQuizMigrations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "CompletedOn",
                table: "StudentQuizzes",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "StudentQuizzes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "StudentQuizQuestions",
                columns: table => new
                {
                    StudentQuizId = table.Column<int>(type: "int", nullable: false),
                    QuestionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentQuizQuestions", x => new { x.StudentQuizId, x.QuestionId });
                    table.ForeignKey(
                        name: "FK_StudentQuizQuestions_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentQuizQuestions_StudentQuizzes_StudentQuizId",
                        column: x => x.StudentQuizId,
                        principalTable: "StudentQuizzes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "HashedPassword",
                value: "AQAAAAIAAYagAAAAEChE6Czce1ctyBc9mOXYBLA+zZcegX6f+Bi0zf52b2hdkeLxfakjcXOBoSCwxHGGPA==");

            migrationBuilder.CreateIndex(
                name: "IX_StudentQuizQuestions_QuestionId",
                table: "StudentQuizQuestions",
                column: "QuestionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StudentQuizQuestions");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "StudentQuizzes");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CompletedOn",
                table: "StudentQuizzes",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "HashedPassword",
                value: "AQAAAAIAAYagAAAAEK0lYyAXoHXbBim/T58dfnWqGF6VTGnmfMjVNfoN685NdHAxt3WTnmpUhtzD+FcAHg==");
        }
    }
}
