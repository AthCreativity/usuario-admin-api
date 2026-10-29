namespace UsuarioAdminApi.Api.Models;

public class PagedResult<T>
{
    public IReadOnlyList<T> Data { get; set; } = [];
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalItems { get; set; }
    public int TotalPages { get; set; }
    public bool HasNextPage { get; set; }
}