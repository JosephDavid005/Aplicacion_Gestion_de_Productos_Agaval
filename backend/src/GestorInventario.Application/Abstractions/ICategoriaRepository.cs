using GestorInventario.Domain.Entities;

namespace GestorInventario.Application.Abstractions;

public interface ICategoriaRepository
{
    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
    Task<Categoria?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Categoria>> GetAllActiveAsync(CancellationToken cancellationToken = default);
}
