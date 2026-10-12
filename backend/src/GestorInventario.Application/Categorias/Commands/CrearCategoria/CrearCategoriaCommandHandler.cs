using GestorInventario.Application.Abstractions;
using GestorInventario.Application.Categorias.Common;
using GestorInventario.Domain.Entities;
using GestorInventario.Domain.Exceptions;
using MediatR;

namespace GestorInventario.Application.Categorias.Commands.CrearCategoria;

public class CrearCategoriaCommandHandler : IRequestHandler<CrearCategoriaCommand, CategoriaDto>
{
    private readonly ICategoriaRepository _categoriaRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CrearCategoriaCommandHandler(ICategoriaRepository categoriaRepository, IUnitOfWork unitOfWork)
    {
        _categoriaRepository = categoriaRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CategoriaDto> Handle(CrearCategoriaCommand request, CancellationToken cancellationToken)
    {
        if (await _categoriaRepository.ExistsByNameAsync(request.Nombre, null, cancellationToken))
        {
            throw new DomainValidationException($"Ya existe una categoría con el nombre '{request.Nombre}'.");
        }

        var categoria = Categoria.Create(request.Nombre, request.Activo);

        await _categoriaRepository.AddAsync(categoria, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return CategoriaDto.FromDomain(categoria);
    }
}
