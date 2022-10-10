using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace FH.ParcelLogistics.BusinessLogic.Entities;

    public partial class Hop
{
    public string HopType { get; set; }
    public string Code { get; set; }

    public string Description { get; set; }

    public int ProcessingDelayMins { get; set; }

    public string LocationName { get; set; }

    public GeoCoordinate LocationCoordinates { get; set; }

}