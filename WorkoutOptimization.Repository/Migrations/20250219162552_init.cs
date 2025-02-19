using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkoutOptimization.Repository.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "GyrosScropeData",
                columns: table => new
                {
                    GyroscopeDataId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    AccelX = table.Column<int>(type: "int", nullable: false),
                    AccelY = table.Column<int>(type: "int", nullable: false),
                    AccelZ = table.Column<int>(type: "int", nullable: false),
                    GyrosX = table.Column<int>(type: "int", nullable: false),
                    GyrosY = table.Column<int>(type: "int", nullable: false),
                    GyrosZ = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GyrosScropeData", x => x.GyroscopeDataId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "GyrosScropeData",
                columns: new[] { "GyroscopeDataId", "AccelX", "AccelY", "AccelZ", "Date", "GyrosX", "GyrosY", "GyrosZ" },
                values: new object[] { 1, 1, 1, 1, new DateTime(2025, 2, 19, 17, 25, 51, 611, DateTimeKind.Local).AddTicks(554), 1, 1, 1 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GyrosScropeData");
        }
    }
}
