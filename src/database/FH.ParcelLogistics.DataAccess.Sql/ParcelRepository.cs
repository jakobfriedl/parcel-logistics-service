namespace FH.ParcelLogistics.DataAccess.Sql;

using System.Collections.Generic;
using System.Data;
using System.Reflection;
using BingMapsRESTToolkit;
using DataAccess.Entities;
using FH.ParcelLogistics.ServiceAgents.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
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
        _geoEncodingAgent = geoEncodingAgent;
    }
    
    public Parcel GetByTrackingId(string trackingId){
        _context.Database.EnsureCreated();

        _logger.LogDebug($"GetByTrackingId: [trackingId:{trackingId}] Get parcel by trackingId");
        try {
            return _context.Parcels
                .Include(_ => _.FutureHops)
                .Include(_ => _.VisitedHops)
                .Single(_ => _.TrackingId == trackingId); 
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

            // Find endpoint of sender and recipient
            var recipientEndpoint = _context.Hops.OfType<Truck>().AsEnumerable().SingleOrDefault(_ => _.Region.Contains(recipientAddress));
            var senderEndpoint = _context.Hops.OfType<Truck>().AsEnumerable().SingleOrDefault(_ => _.Region.Contains(senderAddress));

            _logger.LogDebug($"Submit: Predicting future hops");
            var futureHops = PredictRoute(senderEndpoint, recipientEndpoint).ToList();

            _logger.LogDebug($"Submit: Adding sender and recipient trucks to future hops");
            futureHops.Insert(0, new HopArrival(){
                Code = senderEndpoint.Code,
                Description = senderEndpoint.Description,
                DateTime = DateTime.Now
            });

            futureHops.Add(new HopArrival(){
                Code = recipientEndpoint.Code,
                Description = recipientEndpoint.Description,
                DateTime = futureHops.Last().DateTime.AddMinutes(recipientEndpoint.ProcessingDelayMins)
            });

            parcel.FutureHops = futureHops;
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
            throw new DALException("Update: Parcel is Null");
        }

        _logger.LogDebug($"Update: Updating parcel");
        _context.Parcels.Update(parcel);
        _logger.LogDebug($"Update: Save changes to database");
        _context.SaveChanges();
        return parcel;
    }

    private IList<HopArrival> PredictRoute(Hop hopA, Hop hopB){
        // Find parent warehouse of sender and recipient
        var parentA = Parent(hopA);
        var parentB = Parent(hopB);

        // If parent warehouse is the same, return the parent warehouse
        if (parentA == parentB){
            return new List<HopArrival>() {
                new HopArrival(){
                    Code = parentA.Code,
                    Description = parentA.Description,
                    DateTime = DateTime.Now.AddMinutes(parentA.ProcessingDelayMins)
                }
            }; 
        } else {
            var route = PredictRoute(parentA, parentB);

            var parentArrivalA = new HopArrival(){
                Code = parentA.Code,
                Description = parentA.Description,
                DateTime = DateTime.Now
            };
            var parentArrivalB = new HopArrival(){
                Code = parentB.Code,
                Description = parentB.Description,
                DateTime = route.Last().DateTime.AddMinutes(parentB.ProcessingDelayMins)
            };

            route.Insert(0, parentArrivalA);
            route.Add(parentArrivalB);

            return route;
        }
    }

    private Hop Parent(Hop hop){
        if(hop is null){
            _logger.LogError($"FindParent: [Hop:{hop}] Hop is null");
            throw new DALException("FindParent: Hop is null");
        }

        var parent = _context.Hops.OfType<Warehouse>()
                            .Include(_ => _.NextHops)
                            .ThenInclude(_ => _.Hop)
                            .AsEnumerable()
                            .SingleOrDefault(_ => _.NextHops.Any(y => y.Hop.HopId == hop.HopId));

        if(parent is null){
            _logger.LogError($"FindParent: [Hop {hop}] Parent not found");
            throw new DALException("FindParent: Parent not found");
        }

        return parent;
    }
}
