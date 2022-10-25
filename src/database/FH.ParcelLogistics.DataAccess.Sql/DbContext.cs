namespace FH.ParcelLogistics.DataAccess.Sql;

using System.Diagnostics.CodeAnalysis;
using DataAccess.Entities; 
using Microsoft.EntityFrameworkCore; 

[ExcludeFromCodeCoverage]
public class DbContext : Microsoft.EntityFrameworkCore.DbContext
{
    public virtual DbSet<Parcel> Parcels { get; set; }
    public virtual DbSet<Recipient> Recipients { get; set; }
    public virtual DbSet<Hop> Hops { get; set; }
    public virtual DbSet<HopArrival> HopArrivals { get; set; }
    public virtual DbSet<Truck> Trucks { get; set; }
    public virtual DbSet<Warehouse> Warehouses { get; set; }
    public virtual DbSet<Transferwarehouse> Transferwarehouses { get; set; }
    public virtual DbSet<WarehouseNextHops> WarehouseNextHops { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder){
        optionsBuilder.UseSqlServer();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder){
        modelBuilder.Entity<Parcel>(e => { 
            e.HasKey(_ => _.ParcelId);
            e.Property(_ => _.ParcelId).ValueGeneratedOnAdd();
            e.Property(_ => _.Weight).IsRequired();
            e.HasOne<Recipient>(_ => _.Recipient);
            e.HasOne<Recipient>(_ => _.Sender);
            e.Property(_ => _.TrackingId);
            e.Property(_ => _.State);
            e.Property(_ => _.VisitedHops);
            e.Property(_ => _.FutureHops);

        });

        modelBuilder.Entity<Recipient>(e => {
            e.HasKey(_ => _.RecipientId);
            e.Property(_ => _.RecipientId).ValueGeneratedOnAdd();
            e.Property(_ => _.Name).IsRequired();
            e.Property(_ => _.Street).IsRequired();
            e.Property(_ => _.PostalCode).IsRequired();
            e.Property(_ => _.City).IsRequired();
            e.Property(_ => _.Country).IsRequired();
        });

        modelBuilder.Entity<Hop>(e => {
            e.HasKey(_ => _.HopId);
            e.Property(_ => _.HopId).ValueGeneratedOnAdd();
            e.Property(_ => _.HopType).IsRequired();
            e.Property(_ => _.Code).IsRequired();
            e.Property(_ => _.Description).IsRequired();
            e.Property(_ => _.ProcessingDelayMins).IsRequired();
            e.Property(_ => _.LocationName).IsRequired();
            e.Property(_ => _.LocationCoordinates).IsRequired().HasColumnType("point");
        });

        modelBuilder.Entity<HopArrival>(e => {
            e.HasKey(_ => _.HopArrivalId);
            e.Property(_ => _.HopArrivalId).ValueGeneratedOnAdd();
            e.Property(_ => _.Code).IsRequired();
            e.Property(_ => _.Description).IsRequired();
            e.Property(_ => _.DateTime).IsRequired().HasColumnType("datetime2");
        });

        modelBuilder.Entity<Truck>(e => {
            e.HasBaseType<Hop>();
            e.Property(_ => _.NumberPlate).IsRequired();
            e.Property(_ => _.Region).IsRequired().HasColumnType("geometry");
        });

        modelBuilder.Entity<Warehouse>(e => {
            e.HasBaseType<Hop>();
            e.Property(_ => _.Level).IsRequired();
            e.HasMany<WarehouseNextHops>(_ => _.NextHops);
        });

        modelBuilder.Entity<WarehouseNextHops>(e => {
            e.HasKey(_ => _.WarehouseNextHopsId);
            e.Property(_ => _.WarehouseNextHopsId).ValueGeneratedOnAdd();
            e.Property(_ => _.TraveltimeMins).IsRequired();
            e.HasOne<Hop>(_ => _.Hop);
        });

        modelBuilder.Entity<Transferwarehouse>(e => {
            e.HasBaseType<Hop>();
            e.Property(_ => _.Region).IsRequired().HasColumnType("geometry");
            e.Property(_ => _.LogisticsPartner).IsRequired();
            e.Property(_ => _.LogisticsPartnerUrl).IsRequired();
        });
    }   
}