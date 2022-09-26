namespace KolmeoCodingTest.Core.Domain.Errors;

public class DomainException : Exception
{
    public ErrorType Type { get; }
    
    public DomainException(ErrorType type, string message) : base(message)
    {
        Type = type;
    }
}