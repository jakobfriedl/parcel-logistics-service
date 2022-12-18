namespace FH.ParcelLogistics.DataAccess.Interfaces;

using DataAccess.Entities; 
public interface IHopRepository
{
    void Import(Hop hop);
    Warehouse Export(); 
    Hop GetByCode(string code);
}
