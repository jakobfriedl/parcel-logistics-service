using System.ComponentModel.DataAnnotations;

namespace FH.ParcelLogistics.DataAccess.Entities;
public partial class Recipient
{
    public int RecipientId { get; private set; }
    [Required]
    public string Name { get; set; }
    [Required]
    public string Street { get; set; }
    [Required]
    public string PostalCode { get; set; }
    [Required]
    public string City { get; set; }
    [Required]
    public string Country { get; set; }
}
