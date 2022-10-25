using System.ComponentModel.DataAnnotations;

namespace FH.ParcelLogistics.DataAccess.Entities;
public partial class Warehouse : Hop
{
    [Required]
    public int Level { get; set; }
    [Required]
    public List<WarehouseNextHops> NextHops { get; set; }
}
