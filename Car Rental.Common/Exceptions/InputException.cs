namespace Car_Rental.Common.Exceptions;

public class InputException : ArgumentException
{
    public InputException(string message) : base(message)
    {
    }
}
