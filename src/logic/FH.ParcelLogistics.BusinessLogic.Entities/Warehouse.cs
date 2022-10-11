namespace FH.ParcelLogistics.BusinessLogic.Entities;
public partial class Warehouse : Hop
{
    public int Level { get; set; }
    public List<WarehouseNextHops> NextHops { get; set; }
}
