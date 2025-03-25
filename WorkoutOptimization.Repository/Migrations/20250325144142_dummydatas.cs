using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkoutOptimization.Repository.Migrations
{
    /// <inheritdoc />
    public partial class dummydatas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "70a9df3f-03b8-4420-a6a5-8f713c3efbb2",
                columns: new[] { "ConcurrencyStamp", "EmailConfirmed", "NormalizedEmail", "PasswordHash", "SecurityStamp" },
                values: new object[] { "9de68ce6-726f-461f-9349-9a9092ef9e58", true, "ADMIN@GMAIL.COM", "AQAAAAIAAYagAAAAEGyiuLEnlrodM7cXAwt+089EVRvZ8MZMrw9uG7yzJIZNKvvT1h/ciXPaXg8bF9Thvg==", "39578718-9817-4657-852b-32b29cce7d0b" });

            migrationBuilder.UpdateData(
                table: "GyrosScropeData",
                keyColumn: "GyroscopeDataId",
                keyValue: 1,
                column: "Date",
                value: new DateTime(2025, 3, 25, 15, 41, 41, 689, DateTimeKind.Local).AddTicks(8548));

            migrationBuilder.UpdateData(
                table: "Trainings",
                keyColumn: "TrainingId",
                keyValue: 1,
                columns: new[] { "End", "Start" },
                values: new object[] { new DateTime(2025, 3, 25, 15, 41, 41, 620, DateTimeKind.Local).AddTicks(5679), new DateTime(2025, 3, 25, 15, 41, 41, 620, DateTimeKind.Local).AddTicks(5636) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "70a9df3f-03b8-4420-a6a5-8f713c3efbb2",
                columns: new[] { "ConcurrencyStamp", "EmailConfirmed", "NormalizedEmail", "PasswordHash", "SecurityStamp" },
                values: new object[] { "a0f77a84-c021-4758-81d2-f5c3d8471e8e", false, null, "AQAAAAIAAYagAAAAEFJ8hcxxsa67EyYrh0mA1xt1gaYo6sF0U3qb7WFpDo+bHT1lywRo1aMFRTFemuJ9/Q==", "e4ca0064-6224-46de-a528-35265148b5cd" });

            migrationBuilder.UpdateData(
                table: "GyrosScropeData",
                keyColumn: "GyroscopeDataId",
                keyValue: 1,
                column: "Date",
                value: new DateTime(2025, 3, 25, 15, 37, 52, 23, DateTimeKind.Local).AddTicks(5907));

            migrationBuilder.UpdateData(
                table: "Trainings",
                keyColumn: "TrainingId",
                keyValue: 1,
                columns: new[] { "End", "Start" },
                values: new object[] { new DateTime(2025, 3, 25, 15, 37, 51, 956, DateTimeKind.Local).AddTicks(8177), new DateTime(2025, 3, 25, 15, 37, 51, 956, DateTimeKind.Local).AddTicks(8125) });
        }
    }
}
