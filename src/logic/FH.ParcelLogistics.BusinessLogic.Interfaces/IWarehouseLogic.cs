using FH.ParcelLogistics.BusinessLogic.Entities;

namespace FH.ParcelLogistics.BusinessLogic.Interfaces;

public interface IWarehouseLogic
{
    object ExportWarehouses();
    object ImportWarehouses(Warehouse warehouse);
    object GetWarehouse(string code);
}
