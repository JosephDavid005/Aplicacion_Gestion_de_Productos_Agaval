namespace GestorInventario.Application.Exceptions;

public class ApplicationValidationException : Exception
{
    public ApplicationValidationException(string message) : base(message)
    {
    }
}
