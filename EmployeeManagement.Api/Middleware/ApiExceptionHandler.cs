using EmployeeManagement.Api.Services;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace EmployeeManagement.Api.Middleware;

public sealed class ApiExceptionHandler(
    ILogger<ApiExceptionHandler> logger,
    IProblemDetailsService problemDetailsService) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (statusCode, title, detail) = exception switch
        {
            ResourceNotFoundException => (
                StatusCodes.Status404NotFound,
                "Resource not found",
                exception.Message),
            ConflictException => (
                StatusCodes.Status409Conflict,
                "Conflict",
                exception.Message),
            ArgumentException => (
                StatusCodes.Status400BadRequest,
                "Invalid request",
                exception.Message),
            DbUpdateException updateException when FindSqlError(updateException) is { } sqlError
                && sqlError.Number is 2601 or 2627 => (
                    StatusCodes.Status409Conflict,
                    "Conflict",
                    "A record with the supplied unique value already exists."),
            DbUpdateException updateException when FindSqlError(updateException)?.Number == 547 => (
                StatusCodes.Status409Conflict,
                "Conflict",
                "The operation conflicts with related records."),
            _ => (0, string.Empty, string.Empty)
        };

        if (statusCode == 0)
        {
            logger.LogError(exception, "Unhandled exception processing {Path}", httpContext.Request.Path);
            return false;
        }

        logger.LogWarning(exception, "API request failed with {StatusCode}", statusCode);
        httpContext.Response.StatusCode = statusCode;

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = httpContext.Request.Path
        };
        var wasWritten = await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = problemDetails,
            Exception = exception
        });

        if (!wasWritten)
        {
            httpContext.Response.ContentType = "application/problem+json";
            await JsonSerializer.SerializeAsync(
                httpContext.Response.Body,
                problemDetails,
                cancellationToken: cancellationToken);
        }

        return true;
    }

    private static SqlException? FindSqlError(Exception exception)
    {
        for (Exception? current = exception; current is not null; current = current.InnerException)
        {
            if (current is SqlException sqlException)
            {
                return sqlException;
            }
        }

        return null;
    }
}
