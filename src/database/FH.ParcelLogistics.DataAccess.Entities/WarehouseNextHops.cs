namespace FH.ParcelLogistics.DataAccess.Entities;
public partial class WarehouseNextHops
{
    public int Id { get; set; }
    public int TraveltimeMins { get; set; }
    public Hop Hop { get; set; }
}
