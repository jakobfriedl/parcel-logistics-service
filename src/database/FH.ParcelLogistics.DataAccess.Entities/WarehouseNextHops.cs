using System.ComponentModel.DataAnnotations;

namespace FH.ParcelLogistics.DataAccess.Entities;
public partial class WarehouseNextHops
{
    public int WarehouseNextHopsId { get; private set; }
    [Required]
    public int TraveltimeMins { get; set; }
    [Required]
    public Hop Hop { get; set; }
}
