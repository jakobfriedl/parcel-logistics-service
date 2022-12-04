using System.Diagnostics.CodeAnalysis;

[Serializable]
[ExcludeFromCodeCoverage]
public class BLException : Exception{
    public BLException() { }
    public BLException(string message) : base(message) { }
    public BLException(string message, Exception inner) : base(message, inner) { }
}