namespace FH.ParcelLogistics.DataAccess.Sql;

using System.Diagnostics.CodeAnalysis;
using DataAccess.Entities; 
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

[ExcludeFromCodeCoverage]
public class DbContext : Microsoft.EntityFrameworkCore.DbContext
{
    public virtual DbSet<Parcel> Parcels { get; set; }
    public virtual DbSet<Recipient> Recipients { get; set; }
    public virtual DbSet<Hop> Hops { get; set; }
    public virtual DbSet<HopArrival> HopArrivals { get; set; }
    public virtual DbSet<WarehouseNextHops> WarehouseNextHops { get; set; }

    public DbContext(){
        Database.EnsureCreated();
    }
    public DbContext(DbContextOptions<DbContext> options) : base(options){
        Database.EnsureCreated();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder){
        if(!optionsBuilder.IsConfigured){
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json")
                .Build(); 

            optionsBuilder.UseSqlServer(configuration.GetConnectionString("DBConnection"), o => {
                o.UseNetTopologySuite();
            });
        }   
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder){
        modelBuilder.Entity<Recipient>(e => {
            e.HasKey(x => x.RecipientId);
            e.Property(x => x.RecipientId).ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<Parcel>(e => { 
            e.HasKey(_ => _.ParcelId);
            e.Property(_ => _.ParcelId).ValueGeneratedOnAdd();
            e.Property(_ => _.Weight).IsRequired();
            e.HasOne<Recipient>(_ => _.Recipient); // .WithMany().OnDelete(DeleteBehavior.Cascade); 
            e.HasOne<Recipient>(_ => _.Sender); // .WithMany().OnDelete(DeleteBehavior.Cascade);   
            e.HasMany<HopArrival>(_ => _.VisitedHops); // .WithOne().OnDelete(DeleteBehavior.Cascade);
            e.HasMany<HopArrival>(_ => _.FutureHops);  // .WithOne().OnDelete(DeleteBehavior.Cascade);
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

        modelBuilder.Entity<Hop>()
            .Property(_ => _.LocationCoordinates).HasColumnType("geometry");

        modelBuilder.Entity<Hop>()
            .Property(_ => _.Region).HasColumnType("geometry");

        modelBuilder.Entity<WarehouseNextHops>(e => {
            e.HasKey(_ => _.WarehouseNextHopsId);
            e.Property(_ => _.WarehouseNextHopsId).ValueGeneratedOnAdd();
            e.Property(_ => _.TraveltimeMins).IsRequired();
            e.HasOne<Hop>(_ => _.Hop); //.WithOne().OnDelete(DeleteBehavior.Cascade);
        });
    }   
}