using System.ComponentModel.DataAnnotations;

namespace FH.ParcelLogistics.DataAccess.Entities;

public partial class HopArrival
{
    public int HopArrivalId { get; set; }
    public string Code { get; set; }
    public string Description { get; set; }
    public DateTime DateTime { get; set; }
}
