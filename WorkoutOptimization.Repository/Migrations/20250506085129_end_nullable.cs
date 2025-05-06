using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkoutOptimization.Repository.Migrations
{
    /// <inheritdoc />
    public partial class end_nullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "End",
                table: "Trainings",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

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
                column: "Date",
                value: new DateTime(2025, 5, 6, 10, 51, 28, 639, DateTimeKind.Local).AddTicks(5570));

            migrationBuilder.UpdateData(
                table: "Trainings",
                keyColumn: "TrainingId",
                keyValue: 1,
                columns: new[] { "End", "Start" },
                values: new object[] { new DateTime(2025, 5, 6, 10, 51, 28, 575, DateTimeKind.Local).AddTicks(872), new DateTime(2025, 5, 6, 10, 51, 28, 575, DateTimeKind.Local).AddTicks(827) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "End",
                table: "Trainings",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "70a9df3f-03b8-4420-a6a5-8f713c3efbb2",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "b23eabf9-4629-4bd3-891d-203e48a6bfdf", "AQAAAAIAAYagAAAAEO3VicD+/GHniKjFyo2gvS2g1u7YUp6dmcZN4YF6Chmzu3PyuXc5f6biQ8iCZ6fuHg==", "3ea3d675-ebd6-4cd9-95cb-8475f3d6665f" });

            migrationBuilder.UpdateData(
                table: "GyrosScropeData",
                keyColumn: "GyroscopeDataId",
                keyValue: 1,
                column: "Date",
                value: new DateTime(2025, 4, 28, 17, 9, 9, 427, DateTimeKind.Local).AddTicks(6277));

            migrationBuilder.UpdateData(
                table: "Trainings",
                keyColumn: "TrainingId",
                keyValue: 1,
                columns: new[] { "End", "Start" },
                values: new object[] { new DateTime(2025, 4, 28, 17, 9, 9, 334, DateTimeKind.Local).AddTicks(7450), new DateTime(2025, 4, 28, 17, 9, 9, 334, DateTimeKind.Local).AddTicks(7396) });
        }
    }
}
