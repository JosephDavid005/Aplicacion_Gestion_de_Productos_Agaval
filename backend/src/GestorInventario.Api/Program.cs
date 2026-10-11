using GestorInventario.Api.Middleware;
using GestorInventario.Infrastructure;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// Inyección de dependencias
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Gestor de Inventario API — Agaval",
        Version = "v1",
        Description = "API REST de Gestión de Productos e Inventario construida con Clean Architecture y CQRS (.NET 10)."
    });
});

// MediatR (capa de Application)
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(GestorInventario.Application.AssemblyReference).Assembly);
});

// Infraestructura (PersistenceContext y repositorios)
builder.Services.AddInfrastructure(builder.Configuration);

// CORS para integración con Angular
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularDev", policy =>
    {
        policy.WithOrigins("http://localhost:4200", "http://127.0.0.1:4200")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

// Middleware de manejo global de excepciones
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Swagger UI
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Gestor de Inventario API v1");
    c.RoutePrefix = "swagger";
});

app.UseCors("AllowAngularDev");
app.MapControllers();

// Migración y Seeding automático al inicio delegado a Infrastructure
await app.Services.InitializeDatabaseAsync();

app.Run();

public partial class Program { }
