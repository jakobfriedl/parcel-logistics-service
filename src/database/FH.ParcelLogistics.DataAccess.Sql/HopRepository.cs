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
        _context.Database.EnsureCreated();

        _logger.LogDebug($"CreateHop: Adding hop to set");
        _context.Hops.Add(hop);
        _logger.LogDebug($"CreateHop: Save changes to database"); 
        _context.SaveChanges();
        return hop;
    }

    public Hop UpdateHop(Hop hop){
        _context.Database.EnsureCreated();

        _logger.LogDebug($"UpdateHop: Updating hop");
        _context.Hops.Update(hop);
        _logger.LogDebug($"UpdateHop: Save changes to database"); 
        _context.SaveChanges();
        return hop;
    }

    public IEnumerable<Hop> GetHops(){
        _context.Database.EnsureCreated();

        _logger.LogDebug($"GetHops: Get all hops from database");
        return _context.Hops.ToList();
    }

    public Hop GetByCode(string code){
        _context.Database.EnsureCreated();

        _logger.LogDebug($"GetByCode: [code:{code}] Get hop by code");
        return _context.Hops.Single(_ => _.Code == code);
    }

    public Hop GetById(int id){
        _context.Database.EnsureCreated();

        _logger.LogDebug($"GetById: [id:{id}] Get hop by id");
        return _context.Hops.Find(id);
    }

    public void Import(Hop hop){
        _context.Database.EnsureCreated();

        _logger.LogDebug($"Import: Adding hop to set");
        _context.Hops.Add(hop);
        _logger.LogDebug($"Import: Save changes to database"); 
        _context.SaveChanges();
    }

    public void Export() {
        _context.Database.EnsureCreated();

        throw new NotImplementedException();
    }
}
