namespace FH.ParcelLogistics.DataAccess.Sql;

using ParcelLogistics.DataAccess.Interfaces;
using DataAccess.Entities;
using System.Collections.Generic;
using RandomDataGenerator.FieldOptions;
using RandomDataGenerator.Randomizers;

public class ParcelRepository : IParcelRepository
{
    private readonly DbContext _context;
    public ParcelRepository(DbContext context){
        _context = context;
    }

    public Parcel GetById(int id) => _context.Parcels.Find(id); 
    public Parcel GetByTrackingId(string trackingId) => _context.Parcels.Single(_ => _.TrackingId == trackingId);
    public IEnumerable<Parcel> GetParcels() => _context.Parcels.ToList(); 

    private string GenerateValidTrackingId(){
        var idGenerator = RandomizerFactory.GetRandomizer(new FieldOptionsTextRegex { Pattern = @"^[A-Z0-9]{9}$" });
        return idGenerator.Generate();
    }

    public Parcel Submit(Parcel parcel){
        // generate tracking id
        parcel.TrackingId = GenerateValidTrackingId();
        _context.Parcels.Add(parcel);
        _context.SaveChanges();
        return parcel;
    }
    public Parcel Update(Parcel parcel){
        _context.Parcels.Update(parcel);
        _context.SaveChanges();
        return parcel;
    }

    public bool Delete(int id){
        var toDelete = _context.Parcels.Find(id);
        _context.Parcels.Remove(toDelete);
        _context.SaveChanges();
        return true; 
    }
}
