namespace FH.ParcelLogistics.DataAccess.Sql;

using System.Reflection.Metadata.Ecma335;
using DataAccess.Entities; 

using ParcelLogistics.DataAccess.Interfaces;
public class HopRepository : IHopRepository
{
    private readonly DbContext _context;
    public HopRepository(DbContext context){
        _context = context;
    }

    public bool Delete(int id){
        throw new NotImplementedException();
    }

    public bool Export(){
        throw new NotImplementedException();
    }

    public Hop GetByCode(string code) => _context.Hops.Single(_ => _.Code == code);

    public Warehouse GetById(int id){
        throw new NotImplementedException();
    }

    public bool Import(Warehouse warehouse){
        throw new NotImplementedException();
    }
}
