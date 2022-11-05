namespace FH.ParcelLogistics.DataAccess.Interfaces;

using DataAccess.Entities; 
public interface IHopRepository
{
    // Hop, Truck, Warehouse 
    bool Import(Hop hop);
    Hop CreateHop(Hop hop);
    Hop UpdateHop(Hop hop);
    Hop GetByCode(string code);
    Hop GetById(int id);
    IEnumerable<Hop> GetHops(); 

    // HopArrival
    HopArrival CreateHopArrival(HopArrival hopArrival);
    HopArrival GetHopArrivalByCode(string code);
    HopArrival GetHopArrivalById(int id);

    // WarehouseNextHops
    WarehouseNextHops CreateWarehouseNextHops(WarehouseNextHops warehouseNextHops);
    WarehouseNextHops GetWarehouseNextHopsById(int id);

}
