using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkoutOptimization.Repository.Migrations
{
    /// <inheritdoc />
    public partial class training_active : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "isActive",
                table: "Trainings",
                type: "bit",
                nullable: false,
                defaultValue: false);

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
                columns: new[] { "End", "Start", "isActive" },
                values: new object[] { new DateTime(2025, 4, 28, 17, 9, 9, 334, DateTimeKind.Local).AddTicks(7450), new DateTime(2025, 4, 28, 17, 9, 9, 334, DateTimeKind.Local).AddTicks(7396), false });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isActive",
                table: "Trainings");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "70a9df3f-03b8-4420-a6a5-8f713c3efbb2",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "9de68ce6-726f-461f-9349-9a9092ef9e58", "AQAAAAIAAYagAAAAEGyiuLEnlrodM7cXAwt+089EVRvZ8MZMrw9uG7yzJIZNKvvT1h/ciXPaXg8bF9Thvg==", "39578718-9817-4657-852b-32b29cce7d0b" });

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
    }
}
