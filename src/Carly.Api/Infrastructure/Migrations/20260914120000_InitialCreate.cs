using Carly.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Infrastructure;

#nullable disable

namespace Carly.Api.Infrastructure.Migrations;

[Migration("20260914120000_InitialCreate")]
[DbContext(typeof(CarlyDbContext))]
public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Rentals",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                BookingNumber = table.Column<string>(maxLength: 100, nullable: false),
                RegistrationNumber = table.Column<string>(maxLength: 50, nullable: false),
                Category = table.Column<string>(nullable: false),
                Status = table.Column<string>(nullable: false), 
                CustomerId = table.Column<string>(nullable: true),
                PickupDateTime = table.Column<DateTime>(nullable: true),
                PickupOdometerKm = table.Column<int>(nullable: true),
                ReturnDateTime = table.Column<DateTime>(nullable: true),
                ReturnOdometerKm = table.Column<int>(nullable: true),
                Price = table.Column<decimal>(nullable: true)
            },
            constraints: table => table.PrimaryKey("PK_Rentals", x => x.Id));

        migrationBuilder.CreateIndex(
            name: "IX_Rentals_BookingNumber",
            table: "Rentals",
            column: "BookingNumber",
            unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "Rentals");
    }
}
