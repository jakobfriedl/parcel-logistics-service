namespace FH.ParcelLogistics.DataAccess.Entities;
public partial class Warehouse : Hop
{
    public int WarehouseId { get; set; }
    public int Level { get; set; }
    public List<WarehouseNextHops> NextHops { get; set; }
}
