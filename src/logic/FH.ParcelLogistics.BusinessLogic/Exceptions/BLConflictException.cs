[Serializable]
public class BLConflictException : Exception{
    public BLConflictException() { }
    public BLConflictException(string message) : base(message) { }
    public BLConflictException(string message, Exception inner) : base(message, inner) { }
}