using System.Diagnostics.CodeAnalysis;

[Serializable]
[ExcludeFromCodeCoverage]
public class BLNotFoundException : BLException{
    public BLNotFoundException() { }
    public BLNotFoundException(string message) : base(message) { }
    public BLNotFoundException(string message, Exception inner) : base(message, inner) { }
}