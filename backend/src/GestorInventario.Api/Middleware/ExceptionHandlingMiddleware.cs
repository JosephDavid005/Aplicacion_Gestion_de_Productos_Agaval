using System.Net;
using System.Text.Json;
using GestorInventario.Application.Exceptions;
using GestorInventario.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace GestorInventario.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        _logger.LogError(exception, "Ocurrió una excepción durante el procesamiento de la solicitud: {Message}", exception.Message);

        var (statusCode, title, detail) = exception switch
        {
            DomainValidationException => (
                HttpStatusCode.BadRequest,
                "Error de Validación de Dominio",
                exception.Message
            ),
            ApplicationValidationException => (
                HttpStatusCode.BadRequest,
                "Error de Validación de Negocio",
                exception.Message
            ),
            InvalidOperationException => (
                HttpStatusCode.BadRequest,
                "Operación Inválida",
                exception.Message
            ),
            NotFoundException => (
                HttpStatusCode.NotFound,
                "Recurso No Encontrado",
                exception.Message
            ),
            _ => (
                HttpStatusCode.InternalServerError,
                "Error Interno del Servidor",
                "Ocurrió un error inesperado en el servidor."
            )
        };

        var problemDetails = new ProblemDetails
        {
            Status = (int)statusCode,
            Title = title,
            Detail = detail,
            Instance = context.Request.Path
        };

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = (int)statusCode;

        var json = JsonSerializer.Serialize(problemDetails, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }
}
