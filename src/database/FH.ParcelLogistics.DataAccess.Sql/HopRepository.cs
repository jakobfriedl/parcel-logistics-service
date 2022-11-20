namespace FH.ParcelLogistics.DataAccess.Sql;

using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using DataAccess.Entities;
using Microsoft.Extensions.Logging;
using ParcelLogistics.DataAccess.Interfaces;
public class HopRepository : IHopRepository
{
    private readonly DbContext _context;
    private readonly ILogger<IHopRepository> _logger;
    public HopRepository(DbContext context, ILogger<IHopRepository> logger){
        _context = context;
        _logger = logger;
    }

    // Hop, Truck, Warehouse
    public Hop CreateHop(Hop hop){
        _logger.LogDebug($"CreateHop: Adding hop to set");
        _context.Hops.Add(hop);
        _logger.LogDebug($"CreateHop: Save changes to database"); 
        _context.SaveChanges();
        return hop;
    }

    public Hop UpdateHop(Hop hop){
        _logger.LogDebug($"UpdateHop: Updating hop");
        _context.Hops.Update(hop);
        _logger.LogDebug($"UpdateHop: Save changes to database"); 
        _context.SaveChanges();
        return hop;
    }

    public IEnumerable<Hop> GetHops(){
        _logger.LogDebug($"GetHops: Get all hops from database");
        return _context.Hops.ToList();
    }

    public Hop GetByCode(string code){
        _logger.LogDebug($"GetByCode: [code:{code}] Get hop by code");
        return _context.Hops.Single(_ => _.Code == code);
    }

    public Hop GetById(int id){
        _logger.LogDebug($"GetById: [id:{id}] Get hop by id");
        return _context.Hops.Find(id);
    }

    public void Import(Hop hop){
        _logger.LogDebug($"Import: Adding hop to set");
        _context.Hops.Add(hop);
        _logger.LogDebug($"Import: Save changes to database"); 
        _context.SaveChanges();
    }

    public void Export() {
        throw new NotImplementedException();
    }

    // HopArrival
    public HopArrival CreateHopArrival(HopArrival hopArrival){
        _logger.LogDebug($"CreateHopArrival: Adding hopArrival to set");
        _context.HopArrivals.Add(hopArrival);
        _logger.LogDebug($"CreateHopArrival: Save changes to database"); 
        _context.SaveChanges();
        return hopArrival;
    }

    public HopArrival GetHopArrivalByCode(string code){
        _logger.LogDebug($"GetHopArrivalByCode: [code:{code}] - Get HopArrival by code");
        return _context.HopArrivals.Single(_ => _.Code == code);
    }

    public HopArrival GetHopArrivalById(int id){
        _logger.LogDebug($"GetHopArrivalById: [id:{id}] - Get HopArrival by id");
        return _context.HopArrivals.Find(id);
    } 

    // WarehouseNextHops
    public WarehouseNextHops CreateWarehouseNextHops(WarehouseNextHops warehouseNextHops){
        _logger.LogDebug($"CreateWarehouseNextHops: Adding warehouseNextHops to set");
        _context.WarehouseNextHops.Add(warehouseNextHops);
        _logger.LogDebug($"CreateWarehouseNextHops: Save changes to database"); 
        _context.SaveChanges();
        return warehouseNextHops;
    }

    public WarehouseNextHops GetWarehouseNextHopsById(int id){
        _logger.LogDebug($"GetWarehouseNextHopsById: [id:{id}] - Get warehouseNextHops by id");
        return _context.WarehouseNextHops.Find(id);  
    }
}
