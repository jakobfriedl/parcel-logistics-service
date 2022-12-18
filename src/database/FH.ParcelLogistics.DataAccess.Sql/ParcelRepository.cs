namespace FH.ParcelLogistics.DataAccess.Sql;

using ParcelLogistics.DataAccess.Interfaces;
using DataAccess.Entities;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using System.Reflection;
using System.Data;
using FH.ParcelLogistics.ServiceAgents.Interfaces;
using BingMapsRESTToolkit;

public class ParcelRepository : IParcelRepository
{
    private readonly DbContext _context;
    private readonly ILogger<IParcelRepository> _logger;
    private readonly IGeoEncodingAgent _geoEncodingAgent;

    public ParcelRepository(DbContext context, ILogger<IParcelRepository> logger, IGeoEncodingAgent geoEncodingAgent){
        _context = context;
        _logger = logger;
        _geoEncodingAgent = geoEncodingAgent;
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

        // Get address of sender and recipient
        try{
            var senderAddress = _geoEncodingAgent.EncodeAddress(parcel.Sender);
            var recipientAddress = _geoEncodingAgent.EncodeAddress(parcel.Recipient);
            parcel.FutureHops = PredictRoute(senderAddress, recipientAddress).ToList(); 
        } catch (AddressNotFoundException e){
            _logger.LogError($"Submit: [parcel:{parcel}] Address not found");
            throw new DALException("Submit: Address not found", e);
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
            throw new DALException("Update: Parcel is Null");
        }

        _logger.LogDebug($"Update: Updating parcel");
        _context.Parcels.Update(parcel);
        _logger.LogDebug($"Update: Save changes to database");
        _context.SaveChanges();
        return parcel;
    }

    private IEnumerable<HopArrival> PredictRoute(NetTopologySuite.Geometries.Point senderAddress, NetTopologySuite.Geometries.Point recipientAddress){
        
        var senderTruck = _context.Hops.OfType<Truck>().Where(_ => _.Region.Contains(senderAddress));
        var recipientTruck = _context.Hops.OfType<Truck>().Where(_ => _.Region.Contains(recipientAddress));

        return new List<HopArrival>();
    }
}
