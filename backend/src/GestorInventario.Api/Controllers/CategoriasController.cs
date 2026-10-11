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
}
