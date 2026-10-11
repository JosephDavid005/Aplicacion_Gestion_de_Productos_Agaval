using GestorInventario.Application.Productos.Common;
using GestorInventario.Domain.Enums;
using MediatR;

namespace GestorInventario.Application.Productos.Commands.AjustarStock;

public record AjustarStockCommand(
    int ProductoId,
    TipoMovimiento Tipo,
    int Cantidad
) : IRequest<ProductoDto>;
