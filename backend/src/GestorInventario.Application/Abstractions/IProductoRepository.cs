using GestorInventario.Domain.Entities;

namespace GestorInventario.Application.Abstractions;

public interface IProductoRepository
{
    Task<Producto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Producto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Producto>> GetLowStockAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Producto producto, CancellationToken cancellationToken = default);
    void Update(Producto producto);
    void Delete(Producto producto);
}
