using GestorInventario.Domain.Entities;

namespace GestorInventario.Application.Categorias.Common;

public record CategoriaDto(int Id, string Nombre, bool Activo)
{
    public static CategoriaDto FromDomain(Categoria c) => new(c.Id, c.Nombre, c.Activo);
}
