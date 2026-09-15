using Carly.Api.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

#nullable disable

namespace Carly.Api.Infrastructure.Migrations;

[DbContext(typeof(CarlyDbContext))]
partial class CarlyDbContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasAnnotation("ProductVersion", "10.0.11")
            .HasAnnotation("Relational:MaxIdentifierLength", 64);

        modelBuilder.Entity("Carly.Api.Infrastructure.RentalEntity", entity =>
        {
            entity.Property<string>("BookingNumber")
                .HasMaxLength(100)
                .HasColumnType("TEXT");

            entity.Property<string>("CustomerId").HasColumnType("TEXT");
            entity.Property<Features.Shared.Models.CarCategory>("Category")
                .HasConversion<string>()
                .HasColumnType("TEXT");
            entity.Property<DateTime?>("PickupDateTime").HasColumnType("TEXT");
            entity.Property<int?>("PickupOdometerKm").HasColumnType("INTEGER");
            entity.Property<decimal?>("Price").HasColumnType("TEXT");
            entity.Property<string>("RegistrationNumber")
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnType("TEXT");
            entity.Property<DateTime?>("ReturnDateTime").HasColumnType("TEXT");
            entity.Property<int?>("ReturnOdometerKm").HasColumnType("INTEGER");
            entity.Property<Features.Shared.Models.RentalStatus>("Status")
                .HasConversion<string>()
                .HasColumnType("TEXT");
            entity.HasKey("BookingNumber");
            entity.ToTable("Rentals");
        });
    }
}
