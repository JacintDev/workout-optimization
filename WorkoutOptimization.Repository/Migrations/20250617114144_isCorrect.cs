using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkoutOptimization.Repository.Migrations
{
    /// <inheritdoc />
    public partial class isCorrect : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCorrect",
                table: "Trainings",
                type: "bit",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "70a9df3f-03b8-4420-a6a5-8f713c3efbb2",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "5c9f4b2e-6f26-4f79-b344-c704a90729a4", "AQAAAAIAAYagAAAAEA+S9cDBSw5eVsdiiMyrtF5NX00+VjGzHv7NpNZRsCBmvAG58HeMEQBTZToYAAj1mw==", "325dd98c-b56e-4e73-9f19-add2b71b07f6" });

            migrationBuilder.UpdateData(
                table: "GyrosScropeData",
                keyColumn: "GyroscopeDataId",
                keyValue: 1,
                column: "Date",
                value: new DateTime(2025, 6, 17, 13, 41, 43, 848, DateTimeKind.Local).AddTicks(3182));

            migrationBuilder.UpdateData(
                table: "Trainings",
                keyColumn: "TrainingId",
                keyValue: 1,
                columns: new[] { "End", "IsCorrect", "Start" },
                values: new object[] { new DateTime(2025, 6, 17, 13, 41, 43, 704, DateTimeKind.Local).AddTicks(5755), null, new DateTime(2025, 6, 17, 13, 41, 43, 704, DateTimeKind.Local).AddTicks(5547) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCorrect",
                table: "Trainings");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "70a9df3f-03b8-4420-a6a5-8f713c3efbb2",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "635984c2-b35c-4a7f-aa56-3965c2782ece", "AQAAAAIAAYagAAAAECwqfK5p6711loMnafK5ewfDfTI1ORzE5hi5ynYF7KxaiNMhzgk9PT1ZZyWpRwy7PA==", "e0cbecf6-c5c7-4fc0-bc6d-e1de9ae6e67a" });

            migrationBuilder.UpdateData(
                table: "GyrosScropeData",
                keyColumn: "GyroscopeDataId",
                keyValue: 1,
                column: "Date",
                value: new DateTime(2025, 6, 17, 12, 11, 47, 517, DateTimeKind.Local).AddTicks(7683));

            migrationBuilder.UpdateData(
                table: "Trainings",
                keyColumn: "TrainingId",
                keyValue: 1,
                columns: new[] { "End", "Start" },
                values: new object[] { new DateTime(2025, 6, 17, 12, 11, 47, 454, DateTimeKind.Local).AddTicks(3829), new DateTime(2025, 6, 17, 12, 11, 47, 454, DateTimeKind.Local).AddTicks(3784) });
        }
    }
}
