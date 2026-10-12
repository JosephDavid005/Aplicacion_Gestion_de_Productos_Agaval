using GestorInventario.Domain.Entities;

namespace GestorInventario.Application.Abstractions;

public interface ICategoriaRepository
{
    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> ExistsByNameAsync(string nombre, int? excludeId = null, CancellationToken cancellationToken = default);
    Task<bool> HasProductsAsync(int id, CancellationToken cancellationToken = default);
    Task<Categoria?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Categoria>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Categoria>> GetAllActiveAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Categoria categoria, CancellationToken cancellationToken = default);
    void Update(Categoria categoria);
    void Delete(Categoria categoria);
}
