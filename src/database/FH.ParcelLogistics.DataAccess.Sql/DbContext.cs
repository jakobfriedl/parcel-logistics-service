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
    public virtual DbSet<WebhookResponse> WebhookResponses { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder){}

    public DbContext(DbContextOptions<DbContext> options) : base(options){}

    protected override void OnModelCreating(ModelBuilder modelBuilder){

        modelBuilder.Entity<Hop>(e =>
        {
            e.HasKey(_ => _.HopId);
            e.Property(_ => _.HopId).ValueGeneratedOnAdd();
            e.Property(_ => _.LocationCoordinates).HasColumnType("geometry");

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
               e.HasKey(_ => _.TrackingId);
               e.Property(_ => _.TrackingId).ValueGeneratedOnAdd();
               e.Property(_ => _.TrackingId);
               e.Property(_ => _.Weight).IsRequired();
               e.HasOne<Recipient>(_ => _.Recipient);
               e.HasOne<Recipient>(_ => _.Sender);
               e.HasMany<HopArrival>(_ => _.VisitedHops);
               e.HasMany<HopArrival>(_ => _.FutureHops);
               e.Property(_ => _.State);
           });


        modelBuilder.Entity<WarehouseNextHops>(e =>
        {
            e.HasKey(_ => _.WarehouseNextHopsId);
            e.Property(_ => _.WarehouseNextHopsId).ValueGeneratedOnAdd();
            e.Property(_ => _.TraveltimeMins);
            e.HasOne<Hop>(_ => _.Hop);
        });

        modelBuilder.Entity<Recipient>(e =>
        {
            e.HasKey(_ => _.RecipientId);
            e.Property(_ => _.RecipientId).ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<Truck>()
             .Property(_ => _.Region).HasColumnType("geometry");


        modelBuilder.Entity<Transferwarehouse>()
            .Property(_ => _.Region).HasColumnType("geometry");


        modelBuilder.Entity<HopArrival>(e =>
        {
            e.HasKey(_ => _.HopArrivalId);
            e.Property(_ => _.HopArrivalId).ValueGeneratedOnAdd();
            e.Property(_ => _.Code);
            e.Property(_ => _.Description);
            e.Property(_ => _.DateTime).HasColumnType("datetime");

        });

        modelBuilder.Entity<WebhookResponse>(e => {
            e.HasKey(_ => _.Id);
            e.Property(_ => _.Id).ValueGeneratedOnAdd();
        });
    }   
}