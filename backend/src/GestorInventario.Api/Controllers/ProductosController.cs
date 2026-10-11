using GestorInventario.Application.Productos.Commands.AjustarStock;
using GestorInventario.Application.Productos.Commands.CrearProducto;
using GestorInventario.Application.Productos.Commands.EditarProducto;
using GestorInventario.Application.Productos.Commands.EliminarProducto;
using GestorInventario.Application.Productos.Common;
using GestorInventario.Application.Productos.Queries.ListarProductos;
using GestorInventario.Application.Productos.Queries.ListarProductosStockBajo;
using GestorInventario.Application.Productos.Queries.ObtenerProductoPorId;
using GestorInventario.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GestorInventario.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ProductosController : ControllerBase
{
    private readonly ISender _sender;

    public ProductosController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ProductoDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ProductoDto>>> GetAll(CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new ListarProductosQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpGet("stock-bajo")]
    [ProducesResponseType(typeof(IReadOnlyList<ProductoDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ProductoDto>>> GetLowStock(CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new ListarProductosStockBajoQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ProductoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductoDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new ObtenerProductoPorIdQuery(id), cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ProductoDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProductoDto>> Create([FromBody] CrearProductoCommand command, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ProductoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductoDto>> Update(int id, [FromBody] UpdateProductoRequest request, CancellationToken cancellationToken)
    {
        var command = new EditarProductoCommand(id, request.Nombre, request.Descripcion, request.Precio, request.StockMinimo, request.CategoriaId);
        var result = await _sender.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpPatch("{id:int}/stock")]
    [ProducesResponseType(typeof(ProductoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductoDto>> AdjustStock(int id, [FromBody] AjusteStockRequest request, CancellationToken cancellationToken)
    {
        var command = new AjustarStockCommand(id, request.Tipo, request.Cantidad);
        var result = await _sender.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await _sender.Send(new EliminarProductoCommand(id), cancellationToken);
        return NoContent();
    }
}

public record UpdateProductoRequest(
    string Nombre,
    string? Descripcion,
    decimal Precio,
    int StockMinimo,
    int CategoriaId
);

public record AjusteStockRequest(
    TipoMovimiento Tipo,
    int Cantidad
);
