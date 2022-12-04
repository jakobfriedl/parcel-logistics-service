using System.Diagnostics.CodeAnalysis;

[Serializable]
[ExcludeFromCodeCoverage]
public class DALNotFoundException : DALException{
    public DALNotFoundException() { }
    public DALNotFoundException(string message) : base(message) { }
    public DALNotFoundException(string message, Exception inner) : base(message, inner) { }
}