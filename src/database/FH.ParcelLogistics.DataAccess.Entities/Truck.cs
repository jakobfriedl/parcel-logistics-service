using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NetTopologySuite.Geometries;

namespace FH.ParcelLogistics.DataAccess.Entities;
public partial class Truck : Hop
{
    public Polygon Region { get; set; }
    public string NumberPlate { get; set; }
}
