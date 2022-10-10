using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace FH.ParcelLogistics.BusinessLogic.Entities;
public partial class Warehouse : Hop
{
    public int Level { get; set; }

    public List<WarehouseNextHops> NextHops { get; set; }
}
