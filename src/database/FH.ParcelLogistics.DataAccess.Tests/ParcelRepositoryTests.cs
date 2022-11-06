namespace FH.ParcelLogistics.DataAccess.Tests;

using System.Diagnostics;
using System.Reflection.Emit;
using NUnit.Framework;
using FizzWare.NBuilder;
using FH.ParcelLogistics.DataAccess.Entities;
using Moq;
using FH.ParcelLogistics.DataAccess.Sql;

public class ParcelRepositoryTests
{
    [Test]
    public void SubmitParcel_ReturnsParcel(){
        var parcel = Builder<Parcel>.CreateNew().Build();
        // arrange
        var contextMock = new Mock<DbContext>(); 
        contextMock.Setup(_ => _.Database.EnsureCreated()).Returns(true); 
        contextMock.Setup(x => x.Parcels.Add(parcel));
        contextMock.Setup(x => x.SaveChanges()).Returns(1);
        var context = contextMock.Object;
        var repository = new ParcelRepository(context);

        // act
        var result = repository.Submit(parcel);

        // assert
        Assert.AreEqual(parcel, result);
    }


}