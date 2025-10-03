using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkoutOptimization.Repository.Migrations
{
    /// <inheritdoc />
    public partial class weight : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DailyWeights",
                columns: table => new
                {
                    DailyWeightId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Weight = table.Column<float>(type: "real", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyWeights", x => x.DailyWeightId);
                    table.ForeignKey(
                        name: "FK_DailyWeights_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "70a9df3f-03b8-4420-a6a5-8f713c3efbb2",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "a02ebfe6-dd87-4d21-bbdc-b58e93e67a41", "AQAAAAIAAYagAAAAENe3rFS3z+u4AztPNVtDHrzHn9jpcUFZnmvJZeqiJqMkAAQj6oAsb4vBqay5K1s+rA==", "4e8f1e71-6aa4-43ce-b26e-170149f24368" });

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

            migrationBuilder.CreateIndex(
                name: "IX_DailyWeights_UserId",
                table: "DailyWeights",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DailyWeights");

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
                columns: new[] { "End", "Start" },
                values: new object[] { new DateTime(2025, 6, 17, 13, 41, 43, 704, DateTimeKind.Local).AddTicks(5755), new DateTime(2025, 6, 17, 13, 41, 43, 704, DateTimeKind.Local).AddTicks(5547) });
        }
    }
}
