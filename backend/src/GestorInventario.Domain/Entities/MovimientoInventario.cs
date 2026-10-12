using GestorInventario.Domain.Enums;
using GestorInventario.Domain.Exceptions;

namespace GestorInventario.Domain.Entities;

public class MovimientoInventario
{
    public int Id { get; private set; }
    public int ProductoId { get; private set; }
    public TipoMovimiento TipoMovimiento { get; private set; }
    public int Cantidad { get; private set; }
    public DateTime Fecha { get; private set; }
    public string? Observacion { get; private set; }

    // Navigation property
    public Producto? Producto { get; private set; }

    // Required for EF Core
    private MovimientoInventario()
    {
    }

    public MovimientoInventario(
        int productoId,
        TipoMovimiento tipoMovimiento,
        int cantidad,
        string? observacion = null)
    {
        if (productoId <= 0)
        {
            throw new DomainValidationException("El id del producto debe ser válido.");
        }

        if (cantidad <= 0)
        {
            throw new DomainValidationException("La cantidad debe ser mayor a cero.");
        }

        if (observacion is not null && observacion.Length > 255)
        {
            throw new DomainValidationException("La observación no puede superar los 255 caracteres.");
        }

        ProductoId = productoId;
        TipoMovimiento = tipoMovimiento;
        Cantidad = cantidad;
        Observacion = string.IsNullOrWhiteSpace(observacion) ? null : observacion.Trim();
        Fecha = DateTime.UtcNow;
    }
}
