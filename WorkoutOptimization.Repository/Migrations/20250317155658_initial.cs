using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkoutOptimization.Repository.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GyrosScropeData",
                columns: table => new
                {
                    GyroscopeDataId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccelX = table.Column<int>(type: "int", nullable: false),
                    AccelY = table.Column<int>(type: "int", nullable: false),
                    AccelZ = table.Column<int>(type: "int", nullable: false),
                    GyrosX = table.Column<int>(type: "int", nullable: false),
                    GyrosY = table.Column<int>(type: "int", nullable: false),
                    GyrosZ = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GyrosScropeData", x => x.GyroscopeDataId);
                });

            migrationBuilder.InsertData(
                table: "GyrosScropeData",
                columns: new[] { "GyroscopeDataId", "AccelX", "AccelY", "AccelZ", "Date", "GyrosX", "GyrosY", "GyrosZ" },
                values: new object[] { 1, 1, 1, 1, new DateTime(2025, 3, 17, 16, 56, 58, 305, DateTimeKind.Local).AddTicks(1723), 1, 1, 1 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GyrosScropeData");
        }
    }
}
