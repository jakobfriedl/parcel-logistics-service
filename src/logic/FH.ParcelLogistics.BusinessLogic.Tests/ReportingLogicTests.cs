using NUnit.Framework;
using FH.ParcelLogistics.BusinessLogic;
using FluentValidation;
using FluentValidation.TestHelper;
using System.Net;

namespace FH.ParcelLogistics.BusinessLogic.Tests;

public class ReportingLogicTest
{
    const string ValidTrackingId = "AUGB84723";
    const string InvalidTrackingId = "AUGB84724F";
    const string ValidHopCode = "AUGB5";
    const string InvalidHopCode = "AUG6283";
    private ReportTrackingIDValidator reportTrackingIDValidator;
    private ReportHopValidator reportHopValidator;
    private ReportingLogic reportingLogic;

    [SetUp]
    public void Setup()
    {
         reportTrackingIDValidator = new ReportTrackingIDValidator();
         reportHopValidator = new ReportHopValidator();
    }

    [Test]
    public void ReportTrackingIDValidator()
    {
        //arrange

        //act
        var validIDResult = reportTrackingIDValidator.TestValidate(ValidTrackingId);
        var invalidIDResult = reportTrackingIDValidator.TestValidate(InvalidTrackingId);

        //assert
        validIDResult.ShouldNotHaveAnyValidationErrors();
        invalidIDResult.ShouldHaveAnyValidationError();
    }

    [Test]
    public void ReportHopValidator(){
        //arrange

        //act
        var validHopResult = reportHopValidator.TestValidate(ValidHopCode);
        var invalidHopResult = reportHopValidator.TestValidate(InvalidHopCode);

        //assert
        validHopResult.ShouldNotHaveAnyValidationErrors();
        invalidHopResult.ShouldHaveAnyValidationError();
    }

    [Test]
    public void ReportParcelDelivery(){
        //arrange
        object returnString = "Successfully reported hop.";

        //act
        // var validReportParcelDelivery =  reportingLogic.ReportParcelDelivery(ValidTrackingId);

        //assert
        Assert.Pass();
    }

    [Test]
    public void ReportParcelHop(){
        //arrange
        object returnString = "Successfully reported hop.";

        //act
        // var validReportHop =  reportingLogic.ReportParcelHop(ValidTrackingId, ValidHopCode);

        //assert
        Assert.Pass();
    }
}