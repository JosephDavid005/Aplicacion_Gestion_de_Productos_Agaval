namespace GestorInventario.Domain.Exceptions;

public class DomainValidationException : Exception
{
    public DomainValidationException(string message) : base(message)
    {
    }

    public static void ThrowIfNullOrWhiteSpace(string? value, string paramName, string? message = null)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainValidationException(message ?? $"El campo '{paramName}' es obligatorio y no puede estar vacío.");
        }
    }
}
