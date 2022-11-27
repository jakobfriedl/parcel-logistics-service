namespace FH.ParcelLogistics.ServiceAgents.Tests;
using System;
using System.Net;
using FH.ParcelLogistics.BusinessLogic;
using FH.ParcelLogistics.BusinessLogic.Entities;
using FH.ParcelLogistics.ServiceAgents.Interfaces;
using FizzWare.NBuilder;
using Moq;
using NUnit.Framework;
using RandomDataGenerator.FieldOptions;
using RandomDataGenerator.Randomizers;


public class NominatimEncodingAgentTests
{
    private string GenerateRandomRegex(string pattern)
    {
        var idGenerator = RandomizerFactory.GetRandomizer(new FieldOptionsTextRegex { Pattern = pattern });
        return idGenerator.Generate();
    }

    private Recipient GenerateValidRecipientObject()
    {
        var recipient = Builder<Recipient>.CreateNew()
            .With(x => x.Name = GenerateRandomRegex(@"^[A-ZÄÖÜß][a-zA-Zäöüß -]*"))
            .With(x => x.Country = ("Österreich"))
            .With(x => x.PostalCode = ("AT-1200"))
            .With(x => x.City = ("Wien"))
            .With(x => x.Street = GenerateRandomRegex("Höchstädtplatz 6"))
            .Build();
        return recipient;
    }

    [Test]
    public void Encode_WithValidRecipient_ReturnsValidGeoLocation()
    {
        // arrange
        var recipient = GenerateValidRecipientObject();
        var encodingAgent = new NominatimEncodingAgent();

        // act
        var geoLocation = encodingAgent.EncodeAddress(recipient);

        // assert
        Assert.IsNotNull(geoLocation);
        Assert.IsInstanceOf<GeoCoordinate>(geoLocation);
    }
}
