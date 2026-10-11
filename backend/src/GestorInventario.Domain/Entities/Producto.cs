using GestorInventario.Domain.Enums;
using GestorInventario.Domain.Exceptions;

namespace GestorInventario.Domain.Entities;

public class Producto
{
    public int Id { get; private set; }
    public string Nombre { get; private set; } = string.Empty;
    public string? Descripcion { get; private set; }
    public decimal Precio { get; private set; }
    public int Stock { get; private set; }
    public int StockMinimo { get; private set; }
    public int CategoriaId { get; private set; }
    public DateTime FechaCreacion { get; private set; }

    // Propiedad de navegación para EF Core
    public Categoria? Categoria { get; private set; }

    // Regla de negocio pura (especificada en PDF): Stock bajo ocurre estrictamente si Stock < StockMinimo
    public bool IsLowStock => Stock < StockMinimo;

    // Constructor sin parámetros requerido por EF Core
    private Producto() { }

    private Producto(
        string nombre,
        string? descripcion,
        decimal precio,
        int stock,
        int stockMinimo,
        int categoriaId,
        DateTime fechaCreacion)
    {
        ValidateInvariants(nombre, descripcion, precio, stock, stockMinimo, categoriaId);

        Nombre = nombre.Trim();
        Descripcion = string.IsNullOrWhiteSpace(descripcion) ? null : descripcion.Trim();
        Precio = decimal.Round(precio, 2);
        Stock = stock;
        StockMinimo = stockMinimo;
        CategoriaId = categoriaId;
        FechaCreacion = fechaCreacion;
    }

    public static Producto Create(
        string nombre,
        string? descripcion,
        decimal precio,
        int stock,
        int stockMinimo,
        int categoriaId,
        DateTime? fechaCreacion = null)
    {
        return new Producto(
            nombre,
            descripcion,
            precio,
            stock,
            stockMinimo,
            categoriaId,
            fechaCreacion ?? DateTime.UtcNow
        );
    }

    public void UpdateDetails(
        string nombre,
        string? descripcion,
        decimal precio,
        int stockMinimo,
        int categoriaId)
    {
        ValidateInvariants(nombre, descripcion, precio, Stock, stockMinimo, categoriaId);

        Nombre = nombre.Trim();
        Descripcion = string.IsNullOrWhiteSpace(descripcion) ? null : descripcion.Trim();
        Precio = decimal.Round(precio, 2);
        StockMinimo = stockMinimo;
        CategoriaId = categoriaId;
    }

    public void AdjustStock(TipoMovimiento tipo, int cantidad)
    {
        if (cantidad <= 0)
        {
            throw new DomainValidationException("La cantidad para ajustar el stock debe ser mayor a cero.");
        }

        switch (tipo)
        {
            case TipoMovimiento.Entrada:
                Stock += cantidad;
                break;

            case TipoMovimiento.Salida:
                if (Stock - cantidad < 0)
                {
                    throw new DomainValidationException("Stock insuficiente para realizar la salida solicitada.");
                }
                Stock -= cantidad;
                break;

            default:
                throw new DomainValidationException($"Tipo de movimiento '{tipo}' no reconocido.");
        }
    }

    private static void ValidateInvariants(
        string? nombre,
        string? descripcion,
        decimal precio,
        int stock,
        int stockMinimo,
        int categoriaId)
    {
        DomainValidationException.ThrowIfNullOrWhiteSpace(nombre, nameof(Nombre), "El Nombre del producto es obligatorio.");
        if (nombre!.Trim().Length > 150)
        {
            throw new DomainValidationException("El Nombre del producto no puede superar los 150 caracteres.");
        }

        if (!string.IsNullOrWhiteSpace(descripcion) && descripcion.Trim().Length > 500)
        {
            throw new DomainValidationException("La Descripción no puede superar los 500 caracteres.");
        }

        if (precio <= 0)
        {
            throw new DomainValidationException("El Precio debe ser mayor a cero.");
        }

        if (stock < 0)
        {
            throw new DomainValidationException("El Stock no puede ser negativo.");
        }

        if (stockMinimo < 0)
        {
            throw new DomainValidationException("El StockMinimo no puede ser negativo.");
        }

        if (categoriaId <= 0)
        {
            throw new DomainValidationException("Debe asociar una Categoria válida.");
        }
    }
}
