using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using FH.ParcelLogistics.DataAccess.Entities;
using System.IO;

public class TruckResolver : IValueResolver<FH.ParcelLogistics.BusinessLogic.Entities.Truck, Truck, Geometry>, IValueResolver<Truck, FH.ParcelLogistics.BusinessLogic.Entities.Truck, string>
{
    public Geometry Resolve(FH.ParcelLogistics.BusinessLogic.Entities.Truck source, Truck destination, Geometry destMember, ResolutionContext context)
    {
        var jsonserial = NetTopologySuite.IO.GeoJsonSerializer.CreateDefault();
        var feature = (Feature)jsonserial.Deserialize(new StringReader(source.RegionGeoJson), typeof(Feature));
        return feature.Geometry;
    }
    public string Resolve(Truck source, FH.ParcelLogistics.BusinessLogic.Entities.Truck destination, string destMember, ResolutionContext context)
    {
        Feature feature = new(source.Region, null);

        var serializer = GeoJsonSerializer.CreateDefault();
        StringWriter writer = new();
        serializer.Serialize(writer, feature);
        writer.Flush();
        return writer.ToString();
    }
}