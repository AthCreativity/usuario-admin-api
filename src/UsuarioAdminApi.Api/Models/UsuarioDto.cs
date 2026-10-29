namespace UsuarioAdminApi.Api.Models;

public class UsuarioDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public string Estatus { get; set; } = string.Empty;
}