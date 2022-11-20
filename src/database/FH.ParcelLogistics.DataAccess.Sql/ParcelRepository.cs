namespace FH.ParcelLogistics.DataAccess.Sql;

using ParcelLogistics.DataAccess.Interfaces;
using DataAccess.Entities;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

public class ParcelRepository : IParcelRepository
{
    private readonly DbContext _context;
    private readonly ILogger<IParcelRepository> _logger;
    public ParcelRepository(DbContext context, ILogger<IParcelRepository> logger){
        _context = context;
        _logger = logger;
    }

    public Parcel GetById(int id){
        _logger.LogDebug($"GetById: [id:{id}] Get parcel by id");
        return _context.Parcels.Find(id);
    } 
    public Parcel GetByTrackingId(string trackingId){
        _logger.LogDebug($"GetByTrackingId: [trackingId:{trackingId}] Get parcel by trackingId");
        return _context.Parcels.Single(_ => _.TrackingId == trackingId);
    }
    public IEnumerable<Parcel> GetParcels(){
        _logger.LogDebug($"GetParcels: Get all parcels from database");
        return _context.Parcels.ToList();
    }

    public Parcel Submit(Parcel parcel){
        _logger.LogDebug($"Submit: Adding parcel to set");
        _context.Parcels.Add(parcel);
        _logger.LogDebug($"Submit: Save changes to database");
        _context.SaveChanges();
        return parcel;
    }
    public Parcel Update(Parcel parcel){
        _logger.LogDebug($"Update: Updating parcel");
        _context.Parcels.Update(parcel);
        _logger.LogDebug($"Update: Save changes to database");
        _context.SaveChanges();
        return parcel;
    }
}
