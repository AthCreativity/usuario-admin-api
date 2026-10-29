using Serilog;
using UsuarioAdminApi.Api.Middlewares;
using UsuarioAdminApi.Api.Models;

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog();

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("ConnectionStrings:DefaultConnection no está configurada.");

    builder.Services.AddSingleton(new UsuarioRepository(connectionString));

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "UsuarioAdminApi API v1");
            c.RoutePrefix = "swagger";
        });
    }

    app.UseMiddleware<ExceptionHandlingMiddleware>();
    app.UseHttpsRedirection();
    app.UseAuthorization();
    app.MapControllers();

    app.MapGet("/", () => Results.Ok(new
    {
        message = "API Usuarios en ejecución",
        timestamp = DateTime.UtcNow
    }));

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "La API falló al iniciar.");
    throw;
}
finally
{
    Log.CloseAndFlush();
}