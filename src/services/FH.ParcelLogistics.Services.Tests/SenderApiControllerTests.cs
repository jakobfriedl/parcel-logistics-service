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

public class SenderApiControllerTests
{
    private IMapper _mapper; 
    private readonly DTOs.Parcel _validParcel = new DTOs.Parcel(){
        Weight = 1,
        Recipient = new DTOs.Recipient(){
            Name = "John Doe",
            Street = "Street 1",
            City = "City 1",
            PostalCode = "A-1100",
            Country = "Austria"
        },
        Sender = new DTOs.Recipient(){
            Name = "Jane Doe",
            Street = "Street 2",
            City = "City 2",
            PostalCode = "A-1200",
            Country = "Österreich"
        }
    };

    private readonly DTOs.Parcel _invalidParcel = new DTOs.Parcel(){
        Weight = 0,
        Recipient = new DTOs.Recipient(){
            Name = "John Doe",
            Street = "Street 1",
            City = "City 1",
            PostalCode = "B-2323",
            Country = "Austria"
        },
        Sender = new DTOs.Recipient(){
            Name = "Jane Doe",
            Street = "Street 2",
            City = "City 2",
            PostalCode = "A-1200",
            Country = "Germany"
        }
    };

    [SetUp]
    public void Setup(){
        var config = new AutoMapper.MapperConfiguration(cfg => {
            cfg.AddProfile<HelperProfile>();
            cfg.AddProfile<HopProfile>();
            cfg.AddProfile<ParcelProfile>();
        });
        _mapper = config.CreateMapper();
    } 

    [Test]
    public void SubmitParcel_ValidParcel_Returns201(){
        //arrange
        var idRandomizer = RandomizerFactory.GetRandomizer(new FieldOptionsTextRegex { Pattern = @"^[A-Z0-9]{9}$" });
        var randomTrackingId = idRandomizer.Generate();

        var submissionLogicMock = new Mock<BusinessLogic.Interfaces.ISubmissionLogic>();
        submissionLogicMock.Setup(x => x.SubmitParcel(It.IsAny<BusinessLogic.Entities.Parcel>())).Returns(randomTrackingId);
        var submissionLogic = submissionLogicMock.Object;

        var senderApi = new SenderApiController(_mapper);

        //act
        IActionResult result = senderApi.SubmitParcel(_validParcel);

        //assert
        Assert.AreEqual(201, (result as ObjectResult).StatusCode);
        // Assert.AreEqual(randomTrackingId, ((result as ObjectResult).Value as DTOs.NewParcelInfo).TrackingId);
    }

    [Test]
    public void SubmitParcel_InvalidParcel_Returns400(){
        //arrange
        // var submissionLogicMock = new Mock<BusinessLogic.Interfaces.ISubmissionLogic>();
        // submissionLogicMock.Setup(x => x.SubmitParcel(It.IsAny<BusinessLogic.Entities.Parcel>())).Returns<It.IsAny<Error>()>();
        // var submissionLogic = submissionLogicMock.Object;
        
        var senderApi = new SenderApiController(_mapper);

        //act
        IActionResult result = senderApi.SubmitParcel(_invalidParcel);

        //assert
        Assert.AreEqual(400, (result as ObjectResult).StatusCode);
    }
}