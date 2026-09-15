using Microsoft.EntityFrameworkCore;

namespace Carly.Api.Infrastructure;

public sealed class CarlyDbContext(DbContextOptions<CarlyDbContext> options) : DbContext(options)
{
    public DbSet<RentalEntity> Rentals => Set<RentalEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var rental = modelBuilder.Entity<RentalEntity>();

        // BookingNumber uniquely identifies a rental. RegistrationNumber is a
        // snapshot of the assigned vehicle because vehicle storage is out of scope.
        rental.HasKey(entity => entity.BookingNumber);
        rental.Property(entity => entity.BookingNumber).HasMaxLength(100);
        rental.Property(entity => entity.RegistrationNumber).HasMaxLength(50).IsRequired();
        rental.Property(entity => entity.Category).HasConversion<string>().IsRequired();
        rental.Property(entity => entity.Status).HasConversion<string>().IsRequired();
    }
}

public sealed class RentalEntity
{
    public required string BookingNumber { get; set; }
    public required string RegistrationNumber { get; set; }
    public required Features.Shared.Models.CarCategory Category { get; set; }
    public Features.Shared.Models.RentalStatus Status { get; set; }
    public string? CustomerId { get; set; }
    public DateTime? PickupDateTime { get; set; }
    public int? PickupOdometerKm { get; set; }
    public DateTime? ReturnDateTime { get; set; }
    public int? ReturnOdometerKm { get; set; }
    public decimal? Price { get; set; }
}
