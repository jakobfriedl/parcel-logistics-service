namespace FH.ParcelLogistics.DataAccess.Sql;

using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
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

    public Hop GetHopHierarchy(){
        _context.Database.EnsureCreated();
        _logger.LogDebug($"GetHopHierarchy: Get all hops from database");

        var result = _context.Hops.OfType<Warehouse>()
            .Include(_ => _.NextHops)
            .ThenInclude(_ => _.Hop)
            .ToList()
            .SingleOrDefault(_ => _.Level == 0); // Get root warehouse (level 0)

        if (result is null){
            _logger.LogError($"GetHopHierarchy: Root warehouse not found.");
            throw new DALNotFoundException("Root warehouse not found.");
        }

        return result; 
    }

    public Hop GetByCode(string code){
        _context.Database.EnsureCreated();

        _logger.LogDebug($"GetByCode: [code:{code}] Get hop by code");
        try {
            return _context.Hops.Single(_ => _.Code == code);
        } catch (InvalidOperationException e) {
            _logger.LogError($"GetByCode: [code:{code}] Hop not found");
            throw new DALNotFoundException($"Hop with code {code} not found", e);
        }
    }

    public Hop GetById(int id){
        _context.Database.EnsureCreated();
    
        _logger.LogDebug($"GetById: [id:{id}] Get hop by id");
        try {
            return _context.Hops.Find(id);
        } catch(InvalidOperationException e) {
            _logger.LogError($"GetById: [id:{id}] Hop not found");
            throw new DALNotFoundException($"Hop with id {id} not found", e);
        }
    }

    public void Import(Hop hop){
        _context.Database.EnsureDeleted(); 
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
