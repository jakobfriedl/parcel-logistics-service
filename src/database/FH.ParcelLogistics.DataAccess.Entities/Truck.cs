using System.ComponentModel.DataAnnotations.Schema;

namespace FH.ParcelLogistics.DataAccess.Entities;
public partial class Truck : Hop
{
    public int TruckId { get; set; }
    [Column(TypeName = "Geometry")]
    public NetTopologySuite.Geometries.Geometry Region { get; set; }
    public string NumberPlate { get; set; }
}
