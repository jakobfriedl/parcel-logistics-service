namespace FH.ParcelLogistics.DataAccess.Sql;

using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using DataAccess.Entities;
using ParcelLogistics.DataAccess.Interfaces;
public class HopRepository : IHopRepository
{
    private readonly DbContext _context;
    public HopRepository(DbContext context){
        _context = context;
    }

    // Hop, Truck, Warehouse
    public Hop CreateHop(Hop hop){
        _context.Hops.Add(hop);
        _context.SaveChanges();
        return hop;
    }

    public Hop UpdateHop(Hop hop){
        _context.Hops.Update(hop);
        _context.SaveChanges();
        return hop;
    }

    public IEnumerable<Hop> GetHops() => _context.Hops.ToList();

    public Hop GetByCode(string code) => _context.Hops.Single(_ => _.Code == code);

    public Hop GetById(int id) => _context.Hops.Find(id);

    public bool Import(Hop hop){
        _context.Hops.Add(hop);
        _context.SaveChanges();
        return true;
    }

    // HopArrival
    public HopArrival CreateHopArrival(HopArrival hopArrival){
        _context.HopArrivals.Add(hopArrival);
        _context.SaveChanges();
        return hopArrival;
    }

    public HopArrival GetHopArrivalByCode(string code) => _context.HopArrivals.Single(_ => _.Code == code);

    public HopArrival GetHopArrivalById(int id) => _context.HopArrivals.Find(id);

    // WarehouseNextHops
    public WarehouseNextHops CreateWarehouseNextHops(WarehouseNextHops warehouseNextHops){
        _context.WarehouseNextHops.Add(warehouseNextHops);
        _context.SaveChanges();
        return warehouseNextHops;
    }

    public WarehouseNextHops GetWarehouseNextHopsById(int id) => _context.WarehouseNextHops.Find(id);  
}
