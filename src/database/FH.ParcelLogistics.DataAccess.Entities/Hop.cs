using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FH.ParcelLogistics.DataAccess.Entities;

public partial class Hop
{
    public int HopId { get; private set; }
    [Required]
    public string HopType { get; set; }
    [Required]
    public string Code { get; set; }
    [Required]
    public string Description { get; set; }
    [Required]
    public int ProcessingDelayMins { get; set; }
    [Required]
    public string LocationName { get; set; }
    [Required]
    [Column(TypeName = "Point")]
    public NetTopologySuite.Geometries.Point LocationCoordinates { get; set; }

}