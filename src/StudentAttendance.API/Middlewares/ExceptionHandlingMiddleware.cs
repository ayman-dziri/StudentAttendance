using System.Net;
using System.Text.Json;
using StudentAttendance.src.StudentAttendance.Application.Exceptions;

namespace StudentAttendance.src.StudentAttendance.API.Middlewares;

/// <summary>
/// Middleware global qui intercepte les exceptions et retourne les bons codes HTTP
/// </summary>
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
            _logger.LogError(ex, "Une erreur est survenue: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, message) = exception switch
        {
<<<<<<< HEAD
            AbsenceNotFoundException => (HttpStatusCode.NotFound, exception.Message),
            AbsenceAlreadyJustifiedException => (HttpStatusCode.Conflict, exception.Message),
            InvalidOperationException => (HttpStatusCode.BadRequest, exception.Message),
=======
            ConflictScheduleException => (HttpStatusCode.Conflict, exception.Message),
            InvalidOperationException => (HttpStatusCode.BadRequest, exception.Message),
            FormatException => (HttpStatusCode.BadRequest, "L'identifiant fourni n'est pas valide"),
>>>>>>> origin/feature/scrum-19-conflit-horaire
            _ => (HttpStatusCode.InternalServerError, "Une erreur interne est survenue")
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var response = JsonSerializer.Serialize(new { message });
        await context.Response.WriteAsync(response);
    }
}