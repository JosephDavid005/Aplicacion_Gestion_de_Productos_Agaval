using GestorInventario.Domain.Exceptions;

namespace GestorInventario.Domain.Entities;

public class Categoria
{
    public int Id { get; private set; }
    public string Nombre { get; private set; } = string.Empty;
    public bool Activo { get; private set; }

    // Propiedad de navegación para EF Core
    public ICollection<Producto> Productos { get; private set; } = new List<Producto>();

    // Constructor sin parámetros requerido por EF Core
    private Categoria() { }

    private Categoria(string nombre, bool activo)
    {
        Validate(nombre);
        Nombre = nombre.Trim();
        Activo = activo;
    }

    public static Categoria Create(string nombre, bool activo = true)
    {
        return new Categoria(nombre, activo);
    }

    public void UpdateNombre(string nombre)
    {
        Validate(nombre);
        Nombre = nombre.Trim();
    }

    public void Desactivar() => Activo = false;
    public void Activar() => Activo = true;

    private static void Validate(string? nombre)
    {
        DomainValidationException.ThrowIfNullOrWhiteSpace(nombre, nameof(Nombre), "El Nombre de la categoría es obligatorio.");
        if (nombre!.Trim().Length > 100)
        {
            throw new DomainValidationException("El Nombre de la categoría no puede superar los 100 caracteres.");
        }
    }
}
