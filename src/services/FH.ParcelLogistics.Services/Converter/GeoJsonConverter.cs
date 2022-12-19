using System.Diagnostics.CodeAnalysis;
using System.IO;
using AutoMapper;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;


[ExcludeFromCodeCoverage]
internal class GeoJsonConverter :
        IValueConverter<string, NetTopologySuite.Geometries.Geometry>,
        IValueConverter<NetTopologySuite.Geometries.Geometry, string>,
        ITypeConverter<NetTopologySuite.Geometries.Geometry, string>,
        ITypeConverter<string, NetTopologySuite.Geometries.Geometry>
{
    public NetTopologySuite.Geometries.Geometry Convert(string source, Geometry destination, ResolutionContext context)
    {
       return this.Convert(source, context);
    }

    public string Convert(Geometry sourceMember, string destinationMember, ResolutionContext context)
    {
        return this.Convert(sourceMember, context);
    }

    public string Convert(Geometry sourceMember, ResolutionContext context)
    {
            Feature feature = new(sourceMember, null);

            var serializer = GeoJsonSerializer.CreateDefault();

            StringWriter writer = new();
            serializer.Serialize(writer, feature);
            writer.Flush();

            return writer.ToString();
        
    }

    public Geometry Convert(string sourceMember, ResolutionContext context)
    {
        var serializer = NetTopologySuite.IO.GeoJsonSerializer.CreateDefault();
        var feature = (Feature)serializer.Deserialize(new StringReader(sourceMember), typeof(Feature));
        return feature?.Geometry;
    }
}
