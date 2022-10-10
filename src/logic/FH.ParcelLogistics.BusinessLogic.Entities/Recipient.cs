using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace FH.ParcelLogistics.BusinessLogic.Entities;
public partial class Recipient
{
    public string Name { get; set; }

    public string Street { get; set; }

    public string PostalCode { get; set; }

    public string City { get; set; }

    public string Country { get; set; }
}
