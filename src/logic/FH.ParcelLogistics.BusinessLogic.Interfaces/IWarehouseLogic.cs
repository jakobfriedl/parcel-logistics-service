using FH.ParcelLogistics.BusinessLogic.Entities;

namespace FH.ParcelLogistics.BusinessLogic.Interfaces;

public interface IWarehouseLogic
{
    Hop ExportWarehouses();
    void ImportWarehouses(Warehouse warehouse);
    Hop GetWarehouse(string code);
}
