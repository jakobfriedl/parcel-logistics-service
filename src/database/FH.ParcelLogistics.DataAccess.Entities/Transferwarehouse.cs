using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FH.ParcelLogistics.DataAccess.Entities;
public partial class Transferwarehouse : Hop
{
    public NetTopologySuite.Geometries.Geometry Region { get; set; }
    public string LogisticsPartner { get; set; }
    public string LogisticsPartnerUrl { get; set; }
}
