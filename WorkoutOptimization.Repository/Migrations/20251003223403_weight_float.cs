using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkoutOptimization.Repository.Migrations
{
    /// <inheritdoc />
    public partial class weight_float : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<float>(
                name: "Weight",
                table: "AspNetUsers",
                type: "real",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "70a9df3f-03b8-4420-a6a5-8f713c3efbb2",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp", "Weight" },
                values: new object[] { "238ae3be-9756-49f8-b033-a85e64de0bed", "AQAAAAIAAYagAAAAELtINUUDsAgKyuqUHtufcWOjRPNwza1oNExikO9UJJ5nW8kDP+Cml5UkVjo90clXLw==", "796f285f-cab4-4330-846b-a1eedc378288", null });

            migrationBuilder.UpdateData(
                table: "GyrosScropeData",
                keyColumn: "GyroscopeDataId",
                keyValue: 1,
                column: "Date",
                value: new DateTime(2025, 10, 4, 0, 34, 2, 753, DateTimeKind.Local).AddTicks(9862));

            migrationBuilder.UpdateData(
                table: "Trainings",
                keyColumn: "TrainingId",
                keyValue: 1,
                columns: new[] { "End", "Start" },
                values: new object[] { new DateTime(2025, 10, 4, 0, 34, 2, 667, DateTimeKind.Local).AddTicks(5511), new DateTime(2025, 10, 4, 0, 34, 2, 667, DateTimeKind.Local).AddTicks(5458) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Weight",
                table: "AspNetUsers",
                type: "int",
                nullable: true,
                oldClrType: typeof(float),
                oldType: "real",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "70a9df3f-03b8-4420-a6a5-8f713c3efbb2",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp", "Weight" },
                values: new object[] { "a02ebfe6-dd87-4d21-bbdc-b58e93e67a41", "AQAAAAIAAYagAAAAENe3rFS3z+u4AztPNVtDHrzHn9jpcUFZnmvJZeqiJqMkAAQj6oAsb4vBqay5K1s+rA==", "4e8f1e71-6aa4-43ce-b26e-170149f24368", null });

            migrationBuilder.UpdateData(
                table: "GyrosScropeData",
                keyColumn: "GyroscopeDataId",
                keyValue: 1,
                column: "Date",
                value: new DateTime(2025, 10, 3, 21, 8, 50, 663, DateTimeKind.Local).AddTicks(9443));

            migrationBuilder.UpdateData(
                table: "Trainings",
                keyColumn: "TrainingId",
                keyValue: 1,
                columns: new[] { "End", "Start" },
                values: new object[] { new DateTime(2025, 10, 3, 21, 8, 50, 603, DateTimeKind.Local).AddTicks(1512), new DateTime(2025, 10, 3, 21, 8, 50, 603, DateTimeKind.Local).AddTicks(1458) });
        }
    }
}
