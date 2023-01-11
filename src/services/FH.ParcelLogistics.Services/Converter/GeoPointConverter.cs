using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using FH.ParcelLogistics.BusinessLogic.Entities;
using NetTopologySuite.Geometries;

[ExcludeFromCodeCoverage]
internal class GeoPointConverter :
    IValueConverter<GeoCoordinate, NetTopologySuite.Geometries.Point>,
    IValueConverter<NetTopologySuite.Geometries.Point, GeoCoordinate>,
    ITypeConverter<GeoCoordinate, NetTopologySuite.Geometries.Point>,
    ITypeConverter<NetTopologySuite.Geometries.Point, GeoCoordinate>
{
    public Point Convert(GeoCoordinate source, Point dest, ResolutionContext context)
    {
        return this.Convert(source, context);
    }
    public GeoCoordinate Convert(Point source, GeoCoordinate dest, ResolutionContext context)
    {
        return this.Convert(source, context);
    }

    public GeoCoordinate Convert(Point sourceMember, ResolutionContext context)
    {
        if (sourceMember == null)
            return null;

        return new GeoCoordinate()
        {
            Lat = (double)sourceMember.Coordinate.X,
            Lon = (double)sourceMember.Coordinate.Y
        };
    }

    public Point Convert(GeoCoordinate sourceMember, ResolutionContext context)
    {
        if (sourceMember == null)
            return null;

        return new Point((double)sourceMember.Lat, (double)sourceMember.Lon);
    }
}