namespace FH.ParcelLogistics.Services.Tests;

using System.Reflection.Metadata;
using NUnit.Framework;
using RandomDataGenerator.FieldOptions;
using RandomDataGenerator.Randomizers;
using Moq;
using FH.ParcelLogistics.BusinessLogic;
using FH.ParcelLogistics.Services.Controllers;
using FH.ParcelLogistics.BusinessLogic.Entities;
using FH.ParcelLogistics.Services.MappingProfiles;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Castle.Components.DictionaryAdapter.Xml;
using FizzWare.NBuilder;
using FH.ParcelLogistics.Services.DTOs;
using Microsoft.Extensions.Logging;

[TestFixture]
public class SenderApiControllerTests
{
    private IMapper CreateAutoMapper(){
        var config = new AutoMapper.MapperConfiguration(cfg => {
            cfg.AddProfile<GeoProfile>();
            cfg.AddProfile<HopProfile>();
            cfg.AddProfile<ParcelProfile>();
        });
        return config.CreateMapper();
    }

    [Test]
    public void SubmitParcel_ValidParcel_Returns201(){
        // arrange
        var idGenerator = RandomizerFactory.GetRandomizer(new FieldOptionsTextRegex { Pattern = @"^[A-Z0-9]{9}$" });
        var expectedId = idGenerator.Generate();

        var submissionLogicMock = new Mock<BusinessLogic.Interfaces.ISubmissionLogic>();
        submissionLogicMock.Setup(x => x.SubmitParcel(It.IsAny<BusinessLogic.Entities.Parcel>()))
            .Returns(Builder<BusinessLogic.Entities.Parcel>
                        .CreateNew()
                        .With(x => x.TrackingId = expectedId)
                        .Build());
        
        var submissionLogic = submissionLogicMock.Object;
        var mapper = CreateAutoMapper();
        var logger = new Mock<ILogger<ControllerBase>>().Object;
        var senderApi = new SenderApiController(mapper, submissionLogic, logger);

        // act
        var result = senderApi.SubmitParcel(Builder<DTOs.Parcel>.CreateNew().Build()) as ObjectResult;

        // assert
        Assert.AreEqual(201, result?.StatusCode);
        Assert.AreEqual(expectedId, (result?.Value as NewParcelInfo)?.TrackingId);
    }

    [Test]
    public void SubmitParcel_InvalidParcel_Returns400(){
        // arrange
        var submissionLogicMock = new Mock<BusinessLogic.Interfaces.ISubmissionLogic>();
        submissionLogicMock.Setup(x => x.SubmitParcel(It.IsAny<BusinessLogic.Entities.Parcel>()))
            .Throws<BLValidationException>();
        
        var submissionLogic = submissionLogicMock.Object;
        var mapper = CreateAutoMapper();
        var logger = new Mock<ILogger<ControllerBase>>().Object;
        var senderApi = new SenderApiController(mapper, submissionLogic, logger);

        // act
        var result = senderApi.SubmitParcel(Builder<DTOs.Parcel>
                                                .CreateNew()
                                                .With(x => x.Weight = 0)
                                                .Build()) as ObjectResult;

        //assert
        Assert.AreEqual(400, result?.StatusCode);
        Assert.IsInstanceOf<DTOs.Error>(result?.Value);
    }

    [Test]
    public void SubmitParcel_AddressNotFound_Returns404(){
        // arrange
        var submissionLogicMock = new Mock<BusinessLogic.Interfaces.ISubmissionLogic>();
        submissionLogicMock.Setup(x => x.SubmitParcel(It.IsAny<BusinessLogic.Entities.Parcel>()))
            .Throws<BLNotFoundException>();
        
        var submissionLogic = submissionLogicMock.Object;
        var mapper = CreateAutoMapper();
        var logger = new Mock<ILogger<ControllerBase>>().Object;
        var senderApi = new SenderApiController(mapper, submissionLogic, logger);

        // act
        var result = senderApi.SubmitParcel(Builder<DTOs.Parcel>
                                                .CreateNew()
                                                .With(x => x.Recipient = Builder<DTOs.Recipient>.CreateNew().Build())
                                                .With(x => x.Recipient.Street = "Invalid street")
                                                .Build()) as ObjectResult;
        
        // assert
        Assert.AreEqual(404, result?.StatusCode);
        Assert.IsInstanceOf(typeof(DTOs.Error), result?.Value);
    }
}