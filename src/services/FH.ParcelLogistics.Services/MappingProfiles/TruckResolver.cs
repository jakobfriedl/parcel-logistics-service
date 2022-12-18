using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FH.ParcelLogistics.DataAccess.Entities;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;

namespace FH.ParcelLogistics.Services.MappingProfiles
{
    public class CustomResolver : IValueResolver<BusinessLogic.Entities.Truck, Truck, Geometry>, IValueResolver<Truck, BusinessLogic.Entities.Truck, string>
    {
        public Geometry Resolve(BusinessLogic.Entities.Truck source, Truck destination, Geometry destMember, ResolutionContext context)
        {
            var reader = new GeoJsonReader();
            return reader.Read<Geometry>(source.RegionGeoJson);
        }

        public string Resolve(Truck source, BusinessLogic.Entities.Truck destination, string destMember, ResolutionContext context)
        {
            var writer = new GeoJsonWriter();
            return writer.Write(source.Region);
        }
    }
}