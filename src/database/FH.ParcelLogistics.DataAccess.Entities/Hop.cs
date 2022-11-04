using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FH.ParcelLogistics.DataAccess.Entities;

public partial class Hop
{
    public int HopId { get; private set; }
    public string HopType { get; set; }
    public string Code { get; set; }
    public string Description { get; set; }
    public int ProcessingDelayMins { get; set; }
    public string LocationName { get; set; }
    public NetTopologySuite.Geometries.Point LocationCoordinates { get; set; }

}