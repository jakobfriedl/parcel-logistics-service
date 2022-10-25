using System.ComponentModel.DataAnnotations;

namespace FH.ParcelLogistics.DataAccess.Entities;

public partial class HopArrival
{
    public int HopArrivalId { get; private set; }
    [Required]
    public string Code { get; set; }
    [Required]
    public string Description { get; set; }
    [Required]
    public DateTime DateTime { get; set; }
}
