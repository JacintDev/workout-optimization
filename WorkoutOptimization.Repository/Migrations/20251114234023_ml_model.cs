using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkoutOptimization.Repository.Migrations
{
    /// <inheritdoc />
    public partial class ml_model : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Axis",
                table: "Trainings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "MlModels",
                columns: table => new
                {
                    MlModelId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: true),
                    ModelData = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    DataMin = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataRange = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MlModels", x => x.MlModelId);
                });

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
                columns: new[] { "Axis", "End", "Start" },
                values: new object[] { 0, new DateTime(2025, 11, 15, 0, 40, 22, 1, DateTimeKind.Local).AddTicks(9794), new DateTime(2025, 11, 15, 0, 40, 22, 1, DateTimeKind.Local).AddTicks(9736) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MlModels");

            migrationBuilder.DropColumn(
                name: "Axis",
                table: "Trainings");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "70a9df3f-03b8-4420-a6a5-8f713c3efbb2",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "2c1821cc-afad-43fa-b66c-efdff0300740", "AQAAAAIAAYagAAAAEJc2lEU6ogw9HRMMLP6slHd/zl28thz3p2INoZnovzRddlSM5AkazC9HmttaKGHqYg==", "b86e26f1-9002-4242-ab16-2fbbfab47df1" });

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
    }
}
