using System.ComponentModel.DataAnnotations.Schema;

namespace FH.ParcelLogistics.DataAccess.Entities;

public partial class Hop
{
    public int HopId { get; set; }
    public string HopType { get; set; }
    public string Code { get; set; }
    public string Description { get; set; }
    public int ProcessingDelayMins { get; set; }
    public string LocationName { get; set; }

    [Column(TypeName = "Geometry")]
    public NetTopologySuite.Geometries.Point LocationCoordinates { get; set; }

}