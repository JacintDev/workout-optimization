using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkoutOptimization.Repository.Migrations
{
    /// <inheritdoc />
    public partial class training_cascade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GyrosScropeData_Trainings_TrainingId",
                table: "GyrosScropeData");

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

            migrationBuilder.AddForeignKey(
                name: "FK_GyrosScropeData_Trainings_TrainingId",
                table: "GyrosScropeData",
                column: "TrainingId",
                principalTable: "Trainings",
                principalColumn: "TrainingId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GyrosScropeData_Trainings_TrainingId",
                table: "GyrosScropeData");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "70a9df3f-03b8-4420-a6a5-8f713c3efbb2",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "0d545257-64df-446e-bedf-35a6f335780d", "AQAAAAIAAYagAAAAECu7Hdotc80P1ct+YvqxdNEoC6wzSiDtksQWOmk+F4chTbFO6QDkTd2qaeHCQSN+9w==", "924feecb-8ebe-4203-985d-666fe80148dd" });

            migrationBuilder.UpdateData(
                table: "GyrosScropeData",
                keyColumn: "GyroscopeDataId",
                keyValue: 1,
                column: "Date",
                value: new DateTime(2025, 5, 11, 15, 39, 51, 204, DateTimeKind.Local).AddTicks(409));

            migrationBuilder.UpdateData(
                table: "Trainings",
                keyColumn: "TrainingId",
                keyValue: 1,
                columns: new[] { "End", "Start" },
                values: new object[] { new DateTime(2025, 5, 11, 15, 39, 51, 135, DateTimeKind.Local).AddTicks(7451), new DateTime(2025, 5, 11, 15, 39, 51, 135, DateTimeKind.Local).AddTicks(7408) });

            migrationBuilder.AddForeignKey(
                name: "FK_GyrosScropeData_Trainings_TrainingId",
                table: "GyrosScropeData",
                column: "TrainingId",
                principalTable: "Trainings",
                principalColumn: "TrainingId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
