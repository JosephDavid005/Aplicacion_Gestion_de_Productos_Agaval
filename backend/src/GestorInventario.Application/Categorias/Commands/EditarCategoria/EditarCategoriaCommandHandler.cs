using GestorInventario.Application.Abstractions;
using GestorInventario.Application.Categorias.Common;
using GestorInventario.Application.Exceptions;
using GestorInventario.Domain.Exceptions;
using MediatR;

namespace GestorInventario.Application.Categorias.Commands.EditarCategoria;

public class EditarCategoriaCommandHandler : IRequestHandler<EditarCategoriaCommand, CategoriaDto>
{
    private readonly ICategoriaRepository _categoriaRepository;
    private readonly IUnitOfWork _unitOfWork;

    public EditarCategoriaCommandHandler(ICategoriaRepository categoriaRepository, IUnitOfWork unitOfWork)
    {
        _categoriaRepository = categoriaRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CategoriaDto> Handle(EditarCategoriaCommand request, CancellationToken cancellationToken)
    {
        var categoria = await _categoriaRepository.GetByIdAsync(request.Id, cancellationToken);
        if (categoria == null)
        {
            throw new NotFoundException("Categoria", request.Id);
        }

        if (await _categoriaRepository.ExistsByNameAsync(request.Nombre, request.Id, cancellationToken))
        {
            throw new DomainValidationException($"Ya existe una categoría con el nombre '{request.Nombre}'.");
        }

        categoria.Update(request.Nombre, request.Activo);

        _categoriaRepository.Update(categoria);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return CategoriaDto.FromDomain(categoria);
    }
}
