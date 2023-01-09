namespace FH.ParcelLogistics.DataAccess.Sql;

using System.Collections.Generic;
using System.Data;
using System.Reflection;
using BingMapsRESTToolkit;
using DataAccess.Entities;
using FH.ParcelLogistics.ServiceAgents.Interfaces;
using Microsoft.Extensions.Logging;
using ParcelLogistics.DataAccess.Interfaces;

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

        // Get address of sender and recipient and predict future hops
        try
        {
            var senderAddress = _geoEncodingAgent.EncodeAddress(parcel.Sender);
            var recipientAddress = _geoEncodingAgent.EncodeAddress(parcel.Recipient);

            // Predict future hops
            var senderTruck = _context.Hops.OfType<Truck>().Where(_ => _.Region.Contains(senderAddress)).FirstOrDefault();
            var recipientTruck = _context.Hops.OfType<Truck>().Where(_ => _.Region.Contains(recipientAddress)).FirstOrDefault();
            
            var parentA = _context.WarehouseNextHops.FirstOrDefault(_ => _.Hop.HopId == 2477);

            var futureHops = PredictRoute(senderTruck, recipientTruck).ToList(); 
        } 
        catch (AddressNotFoundException e){
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

    private IList<Hop> PredictRoute(Hop hopA, Hop hopB){
        // Find parent warehouse of sender and recipient
        // var parentA = _context.WarehouseNextHops.FirstOrDefault(_ => _.Hop.HopId == hopA.HopId);
        // var parentB = _context.WarehouseNextHops.FirstOrDefault(_ => _.Hop.HopId == hopB.HopId);

        // if (parentA == parentB){
        //     return new List<Hop>() { parentA }; 
        // } else {
        //     var route = PredictRoute(parentA, parentB);
        //     route.Insert(0, parentA);
        //     route.Add(parentB);

        //     return route;
        // }
        return new List<Hop>();
    }

    // private Hop Parent(Hop hop){
    //     if(hop is null){
    //         _logger.LogError($"FindParent: [Truck:{hop}] Truck is null");
    //         throw new DALException("FindParent: Truck is null");
    //     }

    //     var parent = _context.Hops.Where(_ => _.Contains(hop.Region)).FirstOrDefault();
    //     if(parent is null){
    //         _logger.LogError($"FindParent: [Truck {hop}] Parent not found");
    //         throw new DALException("FindParent: Parent not found");
    //     }

    //     return parent;
    // }
}
