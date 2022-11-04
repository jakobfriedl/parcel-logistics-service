using System.ComponentModel.DataAnnotations;

namespace FH.ParcelLogistics.DataAccess.Entities;
public partial class Warehouse : Hop
{
    public int Level { get; set; }
    public List<WarehouseNextHops> NextHops { get; set; }
}
