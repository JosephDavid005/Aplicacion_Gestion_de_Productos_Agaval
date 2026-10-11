using GestorInventario.Application.Categorias.Common;
using MediatR;

namespace GestorInventario.Application.Categorias.Queries.ListarCategorias;

public record ListarCategoriasQuery : IRequest<IReadOnlyList<CategoriaDto>>;
