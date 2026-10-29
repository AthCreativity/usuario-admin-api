using System.Data.Common;
using Microsoft.Data.SqlClient;
using UsuarioAdminApi.Api.Models;
using Xunit;

namespace UsuarioAdminApi.Tests;

public class UsuarioRepositoryTests
{
    private const string ConnectionString =
        "Server=localhost,1433;Database=UsuariosDb;User Id=sa;Password=MiPasswordSegura2026!Q;Encrypt=False;TrustServerCertificate=True;MultipleActiveResultSets=True;";

    private readonly UsuarioRepository _repository = new(ConnectionString);

    [Fact]
    public async Task SearchAsync_WhenNombreMatches_ReturnsUsers()
    {
        var result = await _repository.SearchAsync("Ana", null, null, 1, 10, CancellationToken.None);

        Assert.NotNull(result.Items);
        Assert.Contains(result.Items, x => x.Nombre.Contains("Ana"));
        Assert.Contains(result.Items, x => x.Correo == "ana@empresa.com");
    }

    [Fact]
    public async Task SearchAsync_WhenCorreoMatches_ReturnsUsers()
    {
        var result = await _repository.SearchAsync(null, "ana@empresa.com", null, 1, 10, CancellationToken.None);

        Assert.NotNull(result.Items);
        Assert.Single(result.Items);
        Assert.Equal("ana@empresa.com", result.Items[0].Correo);
    }

    [Fact]
    public async Task SearchAsync_WhenEstatusMatches_ReturnsUsers()
    {
        var result = await _repository.SearchAsync(null, null, "activo", 1, 10, CancellationToken.None);

        Assert.NotNull(result.Items);
        Assert.NotEmpty(result.Items);
        Assert.All(result.Items, x => Assert.Equal("activo", x.Estatus));
    }

    [Fact]
    public async Task SearchAsync_WhenMultipleFiltersAreCombined_ReturnsMatchingUser()
    {
        var result = await _repository.SearchAsync("Ana", "ana@empresa.com", "activo", 1, 10, CancellationToken.None);

        Assert.NotNull(result.Items);
        Assert.Single(result.Items);
        Assert.Equal("Ana López", result.Items[0].Nombre);
        Assert.Equal("ana@empresa.com", result.Items[0].Correo);
        Assert.Equal("activo", result.Items[0].Estatus);
    }

    [Fact]
    public async Task SearchAsync_WhenNoResultsExist_ReturnsEmpty()
    {
        var result = await _repository.SearchAsync("UsuarioNoExiste", null, null, 1, 10, CancellationToken.None);

        Assert.NotNull(result.Items);
        Assert.Empty(result.Items);
        Assert.Equal(0, result.Total);
    }

    [Fact]
    public async Task SearchAsync_WhenPageAndPageSizeAreValid_ReturnsPagedResult()
    {
        var result = await _repository.SearchAsync(null, null, null, 1, 2, CancellationToken.None);

        Assert.NotNull(result.Items);
        Assert.True(result.Items.Count <= 2);
        Assert.True(result.Total >= 0);
    }

    [Fact]
    public async Task SearchAsync_WhenPageIsInvalid_ThrowsArgumentException()
    {
        await Assert.ThrowsAsync<ArgumentException>(async () =>
            await _repository.SearchAsync(null, null, null, 0, 10, CancellationToken.None));
    }

    [Fact]
    public async Task SearchAsync_WhenPageSizeIsInvalid_ThrowsArgumentException()
    {
        await Assert.ThrowsAsync<ArgumentException>(async () =>
            await _repository.SearchAsync(null, null, null, 1, 51, CancellationToken.None));
    }

    [Fact]
    public async Task SearchAsync_WhenEstatusIsInvalid_ThrowsArgumentException()
    {
        await Assert.ThrowsAsync<ArgumentException>(async () =>
            await _repository.SearchAsync(null, null, "estado_invalido", 1, 10, CancellationToken.None));
    }

    [Fact]
    public async Task SearchAsync_WhenSqlInjectionAttempt_IsHandledAsLiteral()
    {
        var maliciousInput = "Ana'; DROP TABLE dbo.Usuarios; --";

        var result = await _repository.SearchAsync(maliciousInput, null, null, 1, 10, CancellationToken.None);

        Assert.NotNull(result.Items);
        Assert.Empty(result.Items);
    }

    [Fact]
    public async Task SearchAsync_WhenConnectionFails_ThrowsSqlException()
    {
        var brokenRepo = new UsuarioRepository(
            "Server=localhost,1434;Database=UsuariosDb;User Id=sa;Password=bad;Encrypt=False;TrustServerCertificate=True;");

        await Assert.ThrowsAsync<SqlException>(async () =>
            await brokenRepo.SearchAsync("Ana", null, null, 1, 10, CancellationToken.None));
    }
}