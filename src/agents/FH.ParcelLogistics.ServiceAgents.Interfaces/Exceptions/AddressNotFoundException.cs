using System.Diagnostics.CodeAnalysis;

[Serializable]
[ExcludeFromCodeCoverage]
public class AddressNotFoundException : Exception{
    public AddressNotFoundException() { }
    public AddressNotFoundException(string message) : base(message) { }
    public AddressNotFoundException(string message, Exception inner) : base(message, inner) { }
}