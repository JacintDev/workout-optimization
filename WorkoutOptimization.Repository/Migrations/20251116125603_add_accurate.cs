using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkoutOptimization.Repository.Migrations
{
    /// <inheritdoc />
    public partial class add_accurate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "Accurate",
                table: "MlModels",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "70a9df3f-03b8-4420-a6a5-8f713c3efbb2",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "aaa61ae8-7eff-4a44-8377-da52031c535b", "AQAAAAIAAYagAAAAEDKe2gLHgRom+jaFurYuW+fsliendsS4kHgKHjwHVwk700dlkykyXezd+WFiGpbb6w==", "2842dedc-d847-413d-b06c-5030c58bb506" });

            migrationBuilder.UpdateData(
                table: "GyrosScropeData",
                keyColumn: "GyroscopeDataId",
                keyValue: 1,
                column: "Date",
                value: new DateTime(2025, 11, 16, 13, 56, 3, 272, DateTimeKind.Local).AddTicks(4330));

            migrationBuilder.UpdateData(
                table: "Trainings",
                keyColumn: "TrainingId",
                keyValue: 1,
                columns: new[] { "End", "Start" },
                values: new object[] { new DateTime(2025, 11, 16, 13, 56, 3, 202, DateTimeKind.Local).AddTicks(9065), new DateTime(2025, 11, 16, 13, 56, 3, 202, DateTimeKind.Local).AddTicks(9010) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Accurate",
                table: "MlModels");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "70a9df3f-03b8-4420-a6a5-8f713c3efbb2",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "bf2e641d-bf7e-4427-8847-2d92d252037b", "AQAAAAIAAYagAAAAEFsyXIiHJQx99IjX0xHSzpE77dNxaIFR6lE38GMVTnp7ncnV4eCYZxIPjzKK4b+wiw==", "5db6b85f-990a-4800-ba4e-e831b75ca743" });

            migrationBuilder.UpdateData(
                table: "GyrosScropeData",
                keyColumn: "GyroscopeDataId",
                keyValue: 1,
                column: "Date",
                value: new DateTime(2025, 11, 15, 0, 40, 22, 87, DateTimeKind.Local).AddTicks(3035));

            migrationBuilder.UpdateData(
                table: "Trainings",
                keyColumn: "TrainingId",
                keyValue: 1,
                columns: new[] { "End", "Start" },
                values: new object[] { new DateTime(2025, 11, 15, 0, 40, 22, 1, DateTimeKind.Local).AddTicks(9794), new DateTime(2025, 11, 15, 0, 40, 22, 1, DateTimeKind.Local).AddTicks(9736) });
        }
    }
}
