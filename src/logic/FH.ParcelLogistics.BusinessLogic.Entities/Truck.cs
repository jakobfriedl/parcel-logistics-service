using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace FH.ParcelLogistics.BusinessLogic.Entities;
public partial class Truck : Hop
{
    public string RegionGeoJson { get; set; }

    public string NumberPlate { get; set; }
}
