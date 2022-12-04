namespace FH.ParcelLogistics.DataAccess.Interfaces;

using DataAccess.Entities; 
public interface IHopRepository
{
    // Hop, Truck, Warehouse 
    void Import(Hop hop);
    void Export(); 
    Hop CreateHop(Hop hop);
    Hop UpdateHop(Hop hop);
    Hop GetByCode(string code);
    Hop GetById(int id);
    IEnumerable<Hop> GetHops(); 
}
