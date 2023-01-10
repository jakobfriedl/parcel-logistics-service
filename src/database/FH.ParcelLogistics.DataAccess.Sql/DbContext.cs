namespace FH.ParcelLogistics.DataAccess.Sql;

using System.Diagnostics.CodeAnalysis;
using DataAccess.Entities; 
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

[ExcludeFromCodeCoverage]
public class DbContext : Microsoft.EntityFrameworkCore.DbContext
{
    public virtual DbSet<Parcel> Parcels { get; set; }
    public virtual DbSet<Hop> Hops { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder){}

    public DbContext(DbContextOptions<DbContext> options) : base(options){}

    protected override void OnModelCreating(ModelBuilder modelBuilder){

        modelBuilder.Entity<Hop>(e =>
        {
            e.HasKey(c => c.HopId);
            e.Property(c => c.HopId).ValueGeneratedOnAdd();
            e.Property(c => c.LocationCoordinates).HasColumnType("geometry");

            e.HasDiscriminator()
                .HasValue<Warehouse>("Level")
                .HasValue<Transferwarehouse>("Region")
                .HasValue<Transferwarehouse>("LogisticsPartner")
                .HasValue<Transferwarehouse>("LogisticsPartnerUrl")
                .HasValue<Truck>("Region")
                .HasValue<Truck>("PlateNumber");
        });

        modelBuilder.Entity<Parcel>(e =>
           {
               e.HasKey(c => c.TrackingId);
               e.Property(c => c.TrackingId).ValueGeneratedOnAdd();
               e.Property(c => c.TrackingId);
               e.Property(c => c.Weight).IsRequired();
               e.HasOne<Recipient>(c => c.Recipient);
               e.HasOne<Recipient>(c => c.Sender);
               e.HasMany<HopArrival>(c => c.VisitedHops);
               e.HasMany<HopArrival>(c => c.FutureHops);
               e.Property(c => c.State);
           });


        modelBuilder.Entity<WarehouseNextHops>(e =>
        {
            e.HasKey(c => c.WarehouseNextHopsId);
            e.Property(c => c.WarehouseNextHopsId).ValueGeneratedOnAdd();
            e.Property(c => c.TraveltimeMins);
            e.HasOne<Hop>(c => c.Hop);
        });

        modelBuilder.Entity<Recipient>(e =>
        {
            e.HasKey(c => c.RecipientId);
            e.Property(c => c.RecipientId).ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<Truck>()
             .Property(c => c.Region).HasColumnType("geometry");


        modelBuilder.Entity<Transferwarehouse>()
            .Property(c => c.Region).HasColumnType("geometry");


        modelBuilder.Entity<HopArrival>(e =>
        {
            e.HasKey(c => c.HopArrivalId);
            e.Property(c => c.HopArrivalId).ValueGeneratedOnAdd();
            e.Property(c => c.Code);
            e.Property(c => c.Description);
            e.Property(c => c.DateTime).HasColumnType("datetime");
        });
    }   
}