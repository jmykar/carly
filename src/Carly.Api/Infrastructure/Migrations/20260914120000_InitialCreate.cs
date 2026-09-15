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
            constraints: table => table.PrimaryKey("PK_Rentals", x => x.BookingNumber));
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "Rentals");
    }
}
