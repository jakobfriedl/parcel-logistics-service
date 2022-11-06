namespace FH.ParcelLogistics.DataAccess.Tests;

using System.Diagnostics;
using System.Reflection.Emit;
using NUnit.Framework;
using FizzWare.NBuilder;
using FH.ParcelLogistics.DataAccess.Entities;
using Moq;
using FH.ParcelLogistics.DataAccess.Sql;
using Microsoft.EntityFrameworkCore;
using EntityFrameworkCore.Testing.Moq;

public class ParcelRepositoryTests
{
    [Test]
    public void SubmitParcel_ReturnsParcel(){
        // arrange
        Assert.Pass(); 
    }
}