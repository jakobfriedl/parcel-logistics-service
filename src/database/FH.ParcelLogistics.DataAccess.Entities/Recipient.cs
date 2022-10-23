namespace FH.ParcelLogistics.DataAccess.Entities;
public partial class Recipient
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Street { get; set; }
    public string PostalCode { get; set; }
    public string City { get; set; }
    public string Country { get; set; }
}
