using GestorInventario.Application.Abstractions;
using GestorInventario.Domain.Entities;
using GestorInventario.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GestorInventario.Infrastructure.Repositories;

public class ProductoRepository : IProductoRepository
{
    private readonly PersistenceContext _context;

    public ProductoRepository(PersistenceContext context)
    {
        _context = context;
    }

    public async Task<Producto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Productos
            .Include(p => p.Categoria)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Producto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Productos
            .Include(p => p.Categoria)
            .OrderBy(p => p.Nombre)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Producto>> GetLowStockAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Productos
            .Include(p => p.Categoria)
            .Where(p => p.Stock < p.StockMinimo)
            .OrderBy(p => p.Stock)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Producto producto, CancellationToken cancellationToken = default)
    {
        await _context.Productos.AddAsync(producto, cancellationToken);
    }

    public void Update(Producto producto)
    {
        _context.Productos.Update(producto);
    }

    public void Delete(Producto producto)
    {
        _context.Productos.Remove(producto);
    }
}
