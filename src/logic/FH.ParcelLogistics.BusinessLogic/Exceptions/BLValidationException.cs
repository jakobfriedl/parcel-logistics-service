using FH.ParcelLogistics.BusinessLogic.Entities;

[Serializable]
public class BLValidationException : Exception{
    public BLValidationException() { }
    public BLValidationException(string message) : base(message) { }
}