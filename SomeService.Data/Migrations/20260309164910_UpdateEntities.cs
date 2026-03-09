using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SomeService.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "Offices");

            migrationBuilder.DropColumn(
                name: "Phones",
                table: "Offices");

            migrationBuilder.AlterColumn<string>(
                name: "WorkTime",
                table: "Offices",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CountryCode",
                table: "Offices",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AddressApartment",
                table: "Offices",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AddressCity",
                table: "Offices",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AddressHouseNumber",
                table: "Offices",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AddressRegion",
                table: "Offices",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AddressStreet",
                table: "Offices",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Phones",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    OfficeId = table.Column<int>(type: "integer", nullable: false),
                    PhoneNumber = table.Column<string>(type: "text", nullable: false),
                    Additional = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Phones", x => x.id);
                    table.ForeignKey(
                        name: "FK_Phones_Offices_OfficeId",
                        column: x => x.OfficeId,
                        principalTable: "Offices",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_offices_address_city",
                table: "Offices",
                column: "AddressCity");

            migrationBuilder.CreateIndex(
                name: "IX_Phones_OfficeId",
                table: "Phones",
                column: "OfficeId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Phones");

            migrationBuilder.DropIndex(
                name: "ix_offices_address_city",
                table: "Offices");

            migrationBuilder.DropColumn(
                name: "AddressApartment",
                table: "Offices");

            migrationBuilder.DropColumn(
                name: "AddressCity",
                table: "Offices");

            migrationBuilder.DropColumn(
                name: "AddressHouseNumber",
                table: "Offices");

            migrationBuilder.DropColumn(
                name: "AddressRegion",
                table: "Offices");

            migrationBuilder.DropColumn(
                name: "AddressStreet",
                table: "Offices");

            migrationBuilder.AlterColumn<string>(
                name: "WorkTime",
                table: "Offices",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "CountryCode",
                table: "Offices",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Offices",
                type: "jsonb",
                nullable: false,
                defaultValue: "{}");

            migrationBuilder.AddColumn<string>(
                name: "Phones",
                table: "Offices",
                type: "jsonb",
                nullable: true);
        }
    }
}
