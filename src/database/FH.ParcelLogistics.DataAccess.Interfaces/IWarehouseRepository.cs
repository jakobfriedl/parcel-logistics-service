namespace FH.ParcelLogistics.DataAccess.Interfaces;

using DataAccess.Entities; 
public interface IWarehouseRepository
{
    bool Import(Warehouse warehouse);
    bool Export();
    Warehouse GetById(int id);
    Warehouse GetByCode(string code);
    bool Delete(int id); 
}
