using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FH.ParcelLogistics.DataAccess.Entities;
public partial class Transferwarehouse : Hop
{
    [Required]
    [Column(TypeName = "Geometry")]
    public NetTopologySuite.Geometries.Geometry Region { get; set; }
    [Required]
    public string LogisticsPartner { get; set; }
    [Required]
    public string LogisticsPartnerUrl { get; set; }
}
