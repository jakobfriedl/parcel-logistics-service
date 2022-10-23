namespace FH.ParcelLogistics.DataAccess.Sql;

using System.Diagnostics.CodeAnalysis;
using DataAccess.Entities; 
using Microsoft.EntityFrameworkCore; 
using Microsoft.EntityFrameworkCore.SqlServer;

[ExcludeFromCodeCoverage]
public class DbContext : Microsoft.EntityFrameworkCore.DbContext
{
    public virtual DbSet<Parcel> Parcels { get; set; }
    public virtual DbSet<Warehouse> Warehouses { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder){
        modelBuilder.Entity<Parcel>(e => { 
                       
        });
    }   
}