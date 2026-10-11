using GestorInventario.Application.Abstractions;
using GestorInventario.Application.Categorias.Common;
using MediatR;

namespace GestorInventario.Application.Categorias.Queries.ListarCategorias;

public class ListarCategoriasQueryHandler : IRequestHandler<ListarCategoriasQuery, IReadOnlyList<CategoriaDto>>
{
    private readonly ICategoriaRepository _categoriaRepository;

    public ListarCategoriasQueryHandler(ICategoriaRepository categoriaRepository)
    {
        _categoriaRepository = categoriaRepository;
    }

    public async Task<IReadOnlyList<CategoriaDto>> Handle(ListarCategoriasQuery request, CancellationToken cancellationToken)
    {
        var categorias = await _categoriaRepository.GetAllActiveAsync(cancellationToken);
        return categorias.Select(c => CategoriaDto.FromDomain(c)).ToList();
    }
}
