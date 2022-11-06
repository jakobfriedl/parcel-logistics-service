using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NetTopologySuite.Geometries;

namespace FH.ParcelLogistics.DataAccess.Entities;

public partial class Hop
{
    public int HopId { get; set; }
    public string HopType { get; set; }
    public string Code { get; set; }
    public string Description { get; set; }
    public int ProcessingDelayMins { get; set; }
    public string LocationName { get; set; }
    public Point LocationCoordinates { get; set; }

    // Truck
    public Geometry Region { get; set; }
    public string NumberPlate { get; set; }

    // Warehouse
    public int Level { get; set; }
    public List<WarehouseNextHops> NextHops { get; set; }

    // Transferwarehouse
    public string LogisticsPartner { get; set; }
    public string LogisticsPartnerUrl { get; set; }
}