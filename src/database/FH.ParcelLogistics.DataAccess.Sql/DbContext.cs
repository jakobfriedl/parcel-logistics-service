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
        modelBuilder.Entity<Recipient>(e => {
            e.HasKey(x => x.RecipientId);
            e.Property(x => x.RecipientId).ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<Parcel>(e => { 
            e.HasKey(p => p.ParcelId);
            e.Property(p => p.ParcelId).ValueGeneratedOnAdd();
            e.Property(p => p.Weight).IsRequired();
            e.HasOne<Recipient>(p => p.Recipient); 
            e.HasOne<Recipient>(p => p.Sender);   
            e.HasMany<HopArrival>(p => p.VisitedHops);
            e.HasMany<HopArrival>(p => p.FutureHops);  
        });

        modelBuilder.Entity<Hop>()
            .HasDiscriminator<string>("HopType")
            .HasValue<Hop>("Code")
            .HasValue<Hop>("Description")
            .HasValue<Hop>("ProcessingDelayMins")
            .HasValue<Hop>("LocationName")
            .HasValue<Hop>("LocationCoordinates")
            .HasValue<Warehouse>("Level")
            .HasValue<Transferwarehouse>("Region")
            .HasValue<Transferwarehouse>("LogisticPartner")
            .HasValue<Transferwarehouse>("LogisticPartnerUrl")
            .HasValue<Truck>("Region")
            .HasValue<Truck>("NumberPlate");

        modelBuilder.Entity<Warehouse>()
            .HasMany(wh => wh.NextHops);

        modelBuilder.Entity<Truck>()
            .Property(t => t.Region)
            .HasColumnType("geometry")
            .HasColumnName("RegionGeometry");

        modelBuilder.Entity<Transferwarehouse>()
            .Property(twh => twh.Region)
            .HasColumnType("geometry")
            .HasColumnName("RegionGeometry");

        modelBuilder.Entity<WarehouseNextHops>(e => {
            e.HasKey(whnh => whnh.WarehouseNextHopsId);
            e.Property(whnh => whnh.WarehouseNextHopsId).ValueGeneratedOnAdd();
            e.Property(whnh => whnh.TraveltimeMins).IsRequired();
            e.HasOne<Hop>(whnh => whnh.Hop); 
        });
    }   
}