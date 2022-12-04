using System.Diagnostics.CodeAnalysis;
using FH.ParcelLogistics.BusinessLogic.Entities;

[Serializable]
[ExcludeFromCodeCoverage]
public class BLValidationException : BLException{
    public BLValidationException() { }
    public BLValidationException(string message) : base(message) { }
}