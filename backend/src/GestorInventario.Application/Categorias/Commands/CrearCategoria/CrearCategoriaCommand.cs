using GestorInventario.Application.Categorias.Common;
using MediatR;

namespace GestorInventario.Application.Categorias.Commands.CrearCategoria;

public record CrearCategoriaCommand(string Nombre, bool Activo = true) : IRequest<CategoriaDto>;
