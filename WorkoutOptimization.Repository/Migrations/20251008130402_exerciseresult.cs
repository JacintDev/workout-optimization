using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkoutOptimization.Repository.Migrations
{
    /// <inheritdoc />
    public partial class exerciseresult : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExerciseResults",
                columns: table => new
                {
                    ExerciseId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TrainingId = table.Column<int>(type: "int", nullable: false),
                    IsCorrect = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExerciseResults", x => x.ExerciseId);
                    table.ForeignKey(
                        name: "FK_ExerciseResults_Trainings_TrainingId",
                        column: x => x.TrainingId,
                        principalTable: "Trainings",
                        principalColumn: "TrainingId",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_ExerciseResults_TrainingId",
                table: "ExerciseResults",
                column: "TrainingId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExerciseResults");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "70a9df3f-03b8-4420-a6a5-8f713c3efbb2",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "238ae3be-9756-49f8-b033-a85e64de0bed", "AQAAAAIAAYagAAAAELtINUUDsAgKyuqUHtufcWOjRPNwza1oNExikO9UJJ5nW8kDP+Cml5UkVjo90clXLw==", "796f285f-cab4-4330-846b-a1eedc378288" });

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
    }
}
