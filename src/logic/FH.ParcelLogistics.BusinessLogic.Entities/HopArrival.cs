using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace FH.ParcelLogistics.BusinessLogic.Entities;
public partial class HopArrival
{
    public string Code { get; set; }

    public string Description { get; set; }

    public DateTime DateTime { get; set; }
}
