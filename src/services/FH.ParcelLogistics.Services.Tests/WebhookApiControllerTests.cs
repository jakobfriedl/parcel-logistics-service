namespace FH.ParcelLogistics.Services.Tests;

using NUnit.Framework;

using FH.ParcelLogistics.Services.MappingProfiles;
using AutoMapper;

[TestFixture]
public class WebhookApiControllerTests
{
    private IMapper CreateAutoMapper(){
        var config = new AutoMapper.MapperConfiguration(cfg => {
            cfg.AddProfile<GeoProfile>();
            cfg.AddProfile<HopProfile>();
            cfg.AddProfile<ParcelProfile>();
        });
        return config.CreateMapper();
    }
}