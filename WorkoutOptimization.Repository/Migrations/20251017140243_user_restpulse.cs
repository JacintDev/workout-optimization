using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkoutOptimization.Repository.Migrations
{
    /// <inheritdoc />
    public partial class user_restpulse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RestPulse",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "70a9df3f-03b8-4420-a6a5-8f713c3efbb2",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "RestPulse", "SecurityStamp" },
                values: new object[] { "2c1821cc-afad-43fa-b66c-efdff0300740", "AQAAAAIAAYagAAAAEJc2lEU6ogw9HRMMLP6slHd/zl28thz3p2INoZnovzRddlSM5AkazC9HmttaKGHqYg==", null, "b86e26f1-9002-4242-ab16-2fbbfab47df1" });

            migrationBuilder.UpdateData(
                table: "GyrosScropeData",
                keyColumn: "GyroscopeDataId",
                keyValue: 1,
                column: "Date",
                value: new DateTime(2025, 10, 17, 16, 2, 43, 257, DateTimeKind.Local).AddTicks(4406));

            migrationBuilder.UpdateData(
                table: "Trainings",
                keyColumn: "TrainingId",
                keyValue: 1,
                columns: new[] { "End", "Start" },
                values: new object[] { new DateTime(2025, 10, 17, 16, 2, 43, 196, DateTimeKind.Local).AddTicks(7127), new DateTime(2025, 10, 17, 16, 2, 43, 196, DateTimeKind.Local).AddTicks(7072) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RestPulse",
                table: "AspNetUsers");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "70a9df3f-03b8-4420-a6a5-8f713c3efbb2",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "9baf3cf2-3b6b-439f-8c18-5a614ab7f408", "AQAAAAIAAYagAAAAEDB26f9QigSIsH6VJc9lp4ljDSIrXI0qCIwIwgkDJPJHma8GrGmHagzs3iDhM1UjOQ==", "762ecf21-ffb6-4fd5-95e8-a2b4339aff64" });

            migrationBuilder.UpdateData(
                table: "GyrosScropeData",
                keyColumn: "GyroscopeDataId",
                keyValue: 1,
                column: "Date",
                value: new DateTime(2025, 10, 8, 15, 4, 1, 840, DateTimeKind.Local).AddTicks(1662));

            migrationBuilder.UpdateData(
                table: "Trainings",
                keyColumn: "TrainingId",
                keyValue: 1,
                columns: new[] { "End", "Start" },
                values: new object[] { new DateTime(2025, 10, 8, 15, 4, 1, 742, DateTimeKind.Local).AddTicks(9662), new DateTime(2025, 10, 8, 15, 4, 1, 742, DateTimeKind.Local).AddTicks(9609) });
        }
    }
}
