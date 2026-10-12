using GestorInventario.Application.Abstractions;
using GestorInventario.Application.Exceptions;
using MediatR;

namespace GestorInventario.Application.Categorias.Commands.EliminarCategoria;

public class EliminarCategoriaCommandHandler : IRequestHandler<EliminarCategoriaCommand, Unit>
{
    private readonly ICategoriaRepository _categoriaRepository;
    private readonly IUnitOfWork _unitOfWork;

    public EliminarCategoriaCommandHandler(ICategoriaRepository categoriaRepository, IUnitOfWork unitOfWork)
    {
        _categoriaRepository = categoriaRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(EliminarCategoriaCommand request, CancellationToken cancellationToken)
    {
        var categoria = await _categoriaRepository.GetByIdAsync(request.Id, cancellationToken);
        if (categoria == null)
        {
            throw new NotFoundException("Categoria", request.Id);
        }

        if (await _categoriaRepository.HasProductsAsync(request.Id, cancellationToken))
        {
            throw new InvalidOperationException("No se puede eliminar la categoría porque tiene productos asociados.");
        }

        _categoriaRepository.Delete(categoria);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
