using AutoMapper;

using FH.ParcelLogistics.Services.MappingProfiles;

namespace FH.ParcelLogistics.BusinessLogic.Tests;
public class WebhookLogicTests
{
    private IMapper CreateAutoMapper()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<GeoProfile>();
            cfg.AddProfile<ParcelProfile>();
            cfg.AddProfile<HopProfile>();
        });
        return config.CreateMapper();
    }
}