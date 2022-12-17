namespace FH.ParcelLogistics.DataAccess.Sql;

using ParcelLogistics.DataAccess.Interfaces;
using DataAccess.Entities;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using System.Reflection;
using System.Data;

public class ParcelRepository : IParcelRepository
{
    private readonly DbContext _context;
    private readonly ILogger<IParcelRepository> _logger;
    public ParcelRepository(DbContext context, ILogger<IParcelRepository> logger){
        _context = context;
        _logger = logger;
    }

    public Parcel GetById(int id){
        _context.Database.EnsureCreated();

        _logger.LogDebug($"GetById: [id:{id}] Get parcel by id");
        try {
            return _context.Parcels.Find(id);
        } catch(InvalidOperationException e) {
            _logger.LogError($"GetById: [id:{id}] Parcel not found");
            throw new DALNotFoundException($"Parcel with id {id} not found", e);
        }
    } 
    public Parcel GetByTrackingId(string trackingId){
        _context.Database.EnsureCreated();

        _logger.LogDebug($"GetByTrackingId: [trackingId:{trackingId}] Get parcel by trackingId");
        try {
            return _context.Parcels.Single(_ => _.TrackingId == trackingId);
        } catch (InvalidOperationException e) {
            _logger.LogError($"GetByTrackingId: [trackingId:{trackingId}] Parcel not found");
            throw new DALNotFoundException($"Parcel with trackingId {trackingId} not found", e);
        }
    }

    public bool TryGetByTrackingId(string trackingId, out Parcel parcel){
        _context.Database.EnsureCreated();

        _logger.LogDebug($"TryGetByTrackingId: [trackingId:{trackingId}] Try to get parcel by trackingId");
        parcel = _context.Parcels.SingleOrDefault(_ => _.TrackingId == trackingId);
        return parcel != null;
    }

    public IEnumerable<Parcel> GetParcels(){
        _context.Database.EnsureCreated();
    
        _logger.LogDebug($"GetParcels: Get all parcels from database");
        return _context.Parcels.ToList();
    }

    public Parcel Submit(Parcel parcel){
        _context.Database.EnsureCreated();

        if(parcel is null){
            _logger.LogError($"Submit: [parcel:{parcel}] Parcel is null");
            throw new DALException("Submit: Parcel is null");
        }

        _logger.LogDebug($"Submit: Adding parcel to set");
        _context.Parcels.Add(parcel);
        _logger.LogDebug($"Submit: Save changes to database");
        _context.SaveChanges();
        return parcel;
    }
    public Parcel Update(Parcel parcel){
        _context.Database.EnsureCreated();

        if(parcel is null){
            _logger.LogError($"Update: [parcel:{parcel}] Parcel is null");
            throw new DALException("Update: Parcel is Nnull");
        }

        _logger.LogDebug($"Update: Updating parcel");
        _context.Parcels.Update(parcel);
        _logger.LogDebug($"Update: Save changes to database");
        _context.SaveChanges();
        return parcel;
    }
}
