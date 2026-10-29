using System.Text;
using Microsoft.Data.SqlClient;
using UsuarioAdminApi.Api.Models;

namespace UsuarioAdminApi.Api.Models;

public class UsuarioRepository
{
    private readonly string _connectionString;

    public UsuarioRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<(IReadOnlyList<UsuarioDto> Items, int Total)> SearchAsync(
        string? nombre,
        string? correo,
        string? estatus,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        if (page < 1)
            throw new ArgumentException("El parámetro page debe ser mayor o igual a 1.", nameof(page));

        if (pageSize < 1 || pageSize > 50)
            throw new ArgumentException("El parámetro pageSize debe estar entre 1 y 50.", nameof(pageSize));

        var estatusNormalizado = string.IsNullOrWhiteSpace(estatus) ? null : estatus.Trim();

        if (!string.IsNullOrWhiteSpace(estatusNormalizado) &&
            estatusNormalizado != "activo" &&
            estatusNormalizado != "desactivado")
        {
            throw new ArgumentException("El estatus debe ser 'activo' o 'desactivado'.", nameof(estatus));
        }

        var queryBuilder = new StringBuilder();
        var parameters = new List<SqlParameter>();

        queryBuilder.AppendLine("SELECT Id, Nombre, Correo, Estatus");
        queryBuilder.AppendLine("FROM dbo.Usuarios");
        queryBuilder.AppendLine("WHERE 1 = 1");

        if (!string.IsNullOrWhiteSpace(nombre))
        {
            queryBuilder.AppendLine("AND Nombre LIKE @nombre");
            parameters.Add(new SqlParameter("@nombre", $"%{nombre.Trim()}%"));
        }

        if (!string.IsNullOrWhiteSpace(correo))
        {
            queryBuilder.AppendLine("AND Correo LIKE @correo");
            parameters.Add(new SqlParameter("@correo", $"%{correo.Trim()}%"));
        }

        if (!string.IsNullOrWhiteSpace(estatusNormalizado))
        {
            queryBuilder.AppendLine("AND Estatus = @estatus");
            parameters.Add(new SqlParameter("@estatus", estatusNormalizado));
        }

        var countSql = $"SELECT COUNT(*) FROM ({queryBuilder}) AS q";
        var itemsSql = queryBuilder.ToString() + @"
ORDER BY Id
OFFSET @offset ROWS
FETCH NEXT @pageSize ROWS ONLY;";

        var offset = (page - 1) * pageSize;

        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        await using var countCommand = new SqlCommand(countSql, connection);
        AddParameters(countCommand, parameters);

        var total = (int)(await countCommand.ExecuteScalarAsync(cancellationToken) ?? 0);

        await using var itemsCommand = new SqlCommand(itemsSql, connection);
        AddParameters(itemsCommand, parameters);

        itemsCommand.Parameters.AddWithValue("@offset", offset);
        itemsCommand.Parameters.AddWithValue("@pageSize", pageSize);

        await using var reader = await itemsCommand.ExecuteReaderAsync(cancellationToken);
        var items = new List<UsuarioDto>();

        while (await reader.ReadAsync(cancellationToken))
        {
            items.Add(new UsuarioDto
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                Nombre = reader.GetString(reader.GetOrdinal("Nombre")),
                Correo = reader.GetString(reader.GetOrdinal("Correo")),
                Estatus = reader.GetString(reader.GetOrdinal("Estatus"))
            });
        }

        return (items, total);
    }

    private static void AddParameters(SqlCommand command, IEnumerable<SqlParameter> parameters)
    {
        foreach (var parameter in parameters)
        {
            var cloned = new SqlParameter(parameter.ParameterName, parameter.Value ?? DBNull.Value);
            command.Parameters.Add(cloned);
        }
    }
}