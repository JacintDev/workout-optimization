using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkoutOptimization.Repository.Migrations
{
    /// <inheritdoc />
    public partial class trainingset_nullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "TrainingId",
                table: "GyrosScropeData",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "70a9df3f-03b8-4420-a6a5-8f713c3efbb2",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "bcd25b5e-2b90-4cf0-a89b-dfdab68b6360", "AQAAAAIAAYagAAAAEMK0qldb2PNLQbmaKICL2hDpbLzwlrTfacTc2Uh22JSU5QO0uq4XONxnRWQdvy+eMA==", "e473e62a-0c85-408f-a5a8-8951dbaadd22" });

            migrationBuilder.UpdateData(
                table: "GyrosScropeData",
                keyColumn: "GyroscopeDataId",
                keyValue: 1,
                column: "Date",
                value: new DateTime(2025, 3, 25, 14, 42, 30, 53, DateTimeKind.Local).AddTicks(7788));

            migrationBuilder.UpdateData(
                table: "Trainings",
                keyColumn: "TrainingId",
                keyValue: 1,
                columns: new[] { "End", "Start" },
                values: new object[] { new DateTime(2025, 3, 25, 14, 42, 29, 987, DateTimeKind.Local).AddTicks(1680), new DateTime(2025, 3, 25, 14, 42, 29, 987, DateTimeKind.Local).AddTicks(1640) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "TrainingId",
                table: "GyrosScropeData",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "70a9df3f-03b8-4420-a6a5-8f713c3efbb2",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "c4e8158d-bff3-4114-a8a9-13a9c4d44c15", "AQAAAAIAAYagAAAAEBbOtP0WRmxI0dgJu7BiUckkhqnTD2kHIq0vbKHYfL9LCafZaY8GsnDNvrUWWX/pzg==", "f6d72983-81dd-4e75-a0db-3e8e9771d9e6" });

            migrationBuilder.UpdateData(
                table: "GyrosScropeData",
                keyColumn: "GyroscopeDataId",
                keyValue: 1,
                column: "Date",
                value: new DateTime(2025, 3, 25, 14, 28, 57, 537, DateTimeKind.Local).AddTicks(2378));

            migrationBuilder.UpdateData(
                table: "Trainings",
                keyColumn: "TrainingId",
                keyValue: 1,
                columns: new[] { "End", "Start" },
                values: new object[] { new DateTime(2025, 3, 25, 14, 28, 57, 463, DateTimeKind.Local).AddTicks(8291), new DateTime(2025, 3, 25, 14, 28, 57, 463, DateTimeKind.Local).AddTicks(8236) });
        }
    }
}
