using GestorInventario.Application.Categorias.Common;
using MediatR;

namespace GestorInventario.Application.Categorias.Commands.EditarCategoria;

public record EditarCategoriaCommand(int Id, string Nombre, bool Activo) : IRequest<CategoriaDto>;
