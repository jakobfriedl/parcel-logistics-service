namespace FH.ParcelLogistics.ServiceAgents.Tests;
using System;
using System.Net;
using BLEntity = FH.ParcelLogistics.BusinessLogic.Entities;
using DALEntity = FH.ParcelLogistics.DataAccess.Entities;
using FizzWare.NBuilder;
using Moq;
using NUnit.Framework;
using RandomDataGenerator.FieldOptions;
using RandomDataGenerator.Randomizers;
using NetTopologySuite.Geometries;

public class NominatimEncodingAgentTests
{
    private string GenerateRandomRegex(string pattern)
    {
        var idGenerator = RandomizerFactory.GetRandomizer(new FieldOptionsTextRegex { Pattern = pattern });
        return idGenerator.Generate();
    }

    private DALEntity.Recipient GenerateValidRecipientObject()
    {
        var recipient = Builder<DALEntity.Recipient>.CreateNew()
            .With(x => x.Name = GenerateRandomRegex(@"^[A-ZÄÖÜß][a-zA-Zäöüß -]*"))
            .With(x => x.Country = ("Österreich"))
            .With(x => x.PostalCode = ("AT-1200"))
            .With(x => x.City = ("Wien"))
            .With(x => x.Street = GenerateRandomRegex("Höchstädtplatz 6"))
            .Build();
        return recipient;
    }

    private DALEntity.Recipient GenerateInvalidRecipientObject()
    {
        var recipient = Builder<DALEntity.Recipient>.CreateNew()
            .With(x => x.Name = GenerateRandomRegex(@"^[A-ZÄÖÜß][a-zA-Zäöüß -]*"))
            .With(x => x.Country = ("invalid"))
            .With(x => x.PostalCode = ("invalid"))
            .With(x => x.City = ("invalid"))
            .With(x => x.Street = GenerateRandomRegex("invalid"))
            .Build();
        return recipient;
    }

    [Test]
    public void Encode_WithValidRecipient_ReturnsValidGeoLocation()
    {
        // arrange
        var recipient = GenerateValidRecipientObject();
        var encodingAgent = new BingEncodingAgent();

        // act
        var geoLocation = encodingAgent.EncodeAddress(recipient);

        // assert
        Assert.IsNotNull(geoLocation);
        Assert.IsInstanceOf<Point>(geoLocation);
    }

    [Test]
    public void Encode_WithValidRecipient_ReturnsInvalidGeoLocation()
    {
        // arrange
        var recipient = GenerateInvalidRecipientObject();
        var encodingAgent = new BingEncodingAgent();

        // act & assert
        Assert.Throws<Exception>(() => encodingAgent.EncodeAddress(recipient));
    }
}
