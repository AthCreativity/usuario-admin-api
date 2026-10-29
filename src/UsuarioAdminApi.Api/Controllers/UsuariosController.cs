using Microsoft.AspNetCore.Mvc;
using UsuarioAdminApi.Api.Models;
using UsuarioAdminApi.Api.Models;

namespace UsuarioAdminApi.Api.Controllers;

[ApiController]
[Route("api")]
public class UsuariosController : ControllerBase
{
    private readonly UsuarioRepository _repository;

    public UsuariosController(UsuarioRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("usuarios")]
    public async Task<IActionResult> GetUsuarios(
        [FromQuery] string? nombre,
        [FromQuery] string? correo,
        [FromQuery] string? estatus,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var (items, totalItems) = await _repository.SearchAsync(
            nombre,
            correo,
            estatus,
            page,
            pageSize,
            cancellationToken);

        var totalPages = totalItems == 0 ? 0 : (int)Math.Ceiling((double)totalItems / pageSize);

        var result = new PagedResult<UsuarioDto>
        {
            Data = items,
            Page = page,
            PageSize = pageSize,
            TotalItems = totalItems,
            TotalPages = totalPages,
            HasNextPage = page < totalPages
        };

        return Ok(result);
    }
}