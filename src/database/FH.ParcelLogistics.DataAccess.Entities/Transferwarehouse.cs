using System.ComponentModel.DataAnnotations.Schema;

namespace FH.ParcelLogistics.DataAccess.Entities;
public partial class Transferwarehouse : Hop
{
    public int TransferwarehouseId { get; set; }
    [Column(TypeName = "Geometry")]
    public NetTopologySuite.Geometries.Geometry Region { get; set; }
    public string LogisticsPartner { get; set; }
    public string LogisticsPartnerUrl { get; set; }
}
