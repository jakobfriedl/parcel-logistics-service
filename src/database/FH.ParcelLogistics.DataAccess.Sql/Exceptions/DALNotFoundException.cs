[Serializable]
public class DALNotFoundException : Exception{
    public DALNotFoundException() { }
    public DALNotFoundException(string message) : base(message) { }
    public DALNotFoundException(string message, Exception inner) : base(message, inner) { }
}