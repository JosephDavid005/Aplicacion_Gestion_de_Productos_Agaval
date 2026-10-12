using GestorInventario.Application.Categorias.Commands.CrearCategoria;
using GestorInventario.Application.Categorias.Commands.EditarCategoria;
using GestorInventario.Application.Categorias.Commands.EliminarCategoria;
using GestorInventario.Application.Categorias.Common;
using GestorInventario.Application.Categorias.Queries.ListarCategorias;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GestorInventario.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class CategoriasController : ControllerBase
{
    private readonly ISender _sender;

    public CategoriasController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<CategoriaDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<CategoriaDto>>> GetAll(CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new ListarCategoriasQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(CategoriaDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CategoriaDto>> Create([FromBody] CreateCategoriaRequest request, CancellationToken cancellationToken)
    {
        var command = new CrearCategoriaCommand(request.Nombre, request.Activo);
        var result = await _sender.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetAll), new { id = result.Id }, result);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(CategoriaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CategoriaDto>> Update(int id, [FromBody] UpdateCategoriaRequest request, CancellationToken cancellationToken)
    {
        var command = new EditarCategoriaCommand(id, request.Nombre, request.Activo);
        var result = await _sender.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await _sender.Send(new EliminarCategoriaCommand(id), cancellationToken);
        return NoContent();
    }
}

public record CreateCategoriaRequest(string Nombre, bool Activo = true);
public record UpdateCategoriaRequest(string Nombre, bool Activo);
