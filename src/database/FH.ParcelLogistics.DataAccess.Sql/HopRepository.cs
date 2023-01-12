namespace FH.ParcelLogistics.DataAccess.Sql;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Metadata.Ecma335;
using DataAccess.Entities;
using Microsoft.Data.SqlClient;
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

    public Hop GetByCode(string code){
        _context.Database.EnsureCreated();

        _logger.LogDebug($"GetByCode: [code:{code}] Get hop by code");
        try {
            return _context.Hops.SingleOrDefault(_ => _.Code == code);
        } catch (InvalidOperationException e) {
            _logger.LogError($"GetByCode: [code:{code}] Hop not found");
            throw new DALNotFoundException($"Hop with code {code} not found", e);
        }
    }

    [ExcludeFromCodeCoverage]
    public void Import(Hop hop){
        _context.Database.EnsureCreated();

        // Reset Database
        _logger.LogDebug($"Import: Reset database");
        try{
            Reset();
            _logger.LogDebug($"Import: Database reset");
        } catch (SqlException e){
            _logger.LogError($"Import: Database not reset");
            throw new DALException($"Database not reset", e);
        }

        _logger.LogDebug($"Import: Adding hop to set");
        if(hop is null){
            _logger.LogError($"Import: [hop:{hop}] Hop is null");
            throw new DALException("Import: Hop is null");
        }
        try
        {
            _context.Hops.Add(hop);
        }
        catch (Exception e)
        {
            _logger.LogError($"Import: [hop:{hop}] Hop not added to set");
            throw new DALNotFoundException($"Hop not added to set", e);
        }
        _logger.LogDebug($"Import: Save changes to database"); 
        _context.SaveChanges();
    }

    [ExcludeFromCodeCoverage]
    public Warehouse Export()
    {
        _context.Database.EnsureCreated();

        try{
            var result = _context.Hops.OfType<Warehouse>()
                .Include(wh => wh.NextHops)
                .ThenInclude(nxh => nxh.Hop)
                .AsEnumerable()
                .SingleOrDefault(w => w.Level == 0);

            return result;

        }
        catch (Exception e)
        {
            _logger.LogError($"Export: Hop not exported");
            throw new DALNotFoundException($"Hop not exported", e);
        }
    }
    
    private void Reset(){
        _context.Database.ExecuteSqlRaw("DELETE FROM Hops"); 
        _context.Database.ExecuteSqlRaw("DELETE FROM WarehouseNextHops");
        _context.Database.ExecuteSqlRaw("DELETE FROM HopArrival");
        _context.Database.ExecuteSqlRaw("DELETE FROM Parcels");
        _context.Database.ExecuteSqlRaw("DELETE FROM Recipient");
    }
}
