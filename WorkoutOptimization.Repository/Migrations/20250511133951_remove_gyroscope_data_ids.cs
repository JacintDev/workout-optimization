using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkoutOptimization.Repository.Migrations
{
    /// <inheritdoc />
    public partial class remove_gyroscope_data_ids : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GyrosScropeData_AspNetUsers_UserId",
                table: "GyrosScropeData");

            migrationBuilder.DropIndex(
                name: "IX_GyrosScropeData_UserId",
                table: "GyrosScropeData");

            migrationBuilder.DropColumn(
                name: "UserId",
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "GyrosScropeData",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "70a9df3f-03b8-4420-a6a5-8f713c3efbb2",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "ab08e4fe-9966-4eaa-a66f-7e327439fccf", "AQAAAAIAAYagAAAAELzlen/ai1LVLpuj1JoWxZ0R/w3NukzmvRqupL59U7KIwpEh9kMglZHFD6md/nFc9w==", "c981f761-b77d-4ff1-a9d7-8193451f804f" });

            migrationBuilder.UpdateData(
                table: "GyrosScropeData",
                keyColumn: "GyroscopeDataId",
                keyValue: 1,
                columns: new[] { "Date", "UserId" },
                values: new object[] { new DateTime(2025, 5, 6, 10, 51, 28, 639, DateTimeKind.Local).AddTicks(5570), "70a9df3f-03b8-4420-a6a5-8f713c3efbb2" });

            migrationBuilder.UpdateData(
                table: "Trainings",
                keyColumn: "TrainingId",
                keyValue: 1,
                columns: new[] { "End", "Start" },
                values: new object[] { new DateTime(2025, 5, 6, 10, 51, 28, 575, DateTimeKind.Local).AddTicks(872), new DateTime(2025, 5, 6, 10, 51, 28, 575, DateTimeKind.Local).AddTicks(827) });

            migrationBuilder.CreateIndex(
                name: "IX_GyrosScropeData_UserId",
                table: "GyrosScropeData",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_GyrosScropeData_AspNetUsers_UserId",
                table: "GyrosScropeData",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
