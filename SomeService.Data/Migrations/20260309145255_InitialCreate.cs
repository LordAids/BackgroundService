using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SomeService.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Offices",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: true),
                    CityCode = table.Column<int>(type: "integer", nullable: false),
                    Uuid = table.Column<string>(type: "text", nullable: true),
                    Type = table.Column<int>(type: "integer", nullable: true),
                    CountryCode = table.Column<string>(type: "text", nullable: true),
                    WorkTime = table.Column<string>(type: "text", nullable: true),
                    address = table.Column<string>(type: "jsonb", nullable: false),
                    coordinates = table.Column<string>(type: "jsonb", nullable: false),
                    phones = table.Column<string>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Offices", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_offices_city_code",
                table: "Offices",
                column: "CityCode");

            migrationBuilder.CreateIndex(
                name: "ix_offices_code",
                table: "Offices",
                column: "Code");

            migrationBuilder.CreateIndex(
                name: "ix_offices_uuid",
                table: "Offices",
                column: "Uuid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Offices");
        }
    }
}
