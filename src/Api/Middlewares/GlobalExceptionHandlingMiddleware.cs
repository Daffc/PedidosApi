using FluentValidation;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

using Domain.Exceptions;

namespace Api.Middlewares;

public sealed class GlobalExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

    public GlobalExceptionHandlingMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlingMiddleware> logger)
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
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                $"Erro durante processamento da requisição {context.TraceIdentifier} {context.Request.Method} {context.Request.Path}"
            );

            await HandleExceptionAsync(
                context,
                exception
            );
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        int statusCode;
        string title;
        string message;
        switch (exception)
        {
            case DomainException:
                statusCode = StatusCodes.Status400BadRequest;
                title = "Bad Request";
                message = exception.Message;
                break;
            case ValidationException:
                statusCode = StatusCodes.Status400BadRequest;
                title = "Validation Error";
                message = exception.Message;
                break;
            case KeyNotFoundException:
                statusCode = StatusCodes.Status404NotFound;
                title = "Not Found";
                message = exception.Message;
                break;
            case ArgumentException:
                statusCode = StatusCodes.Status400BadRequest;
                title = "Bad Request";
                message = exception.Message;
                break;
            default:
                statusCode = StatusCodes.Status500InternalServerError;
                title = "Internal Server Error";
                message = "Ocorreu um erro interno.";
                break;
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        var problemDetails = new ProblemDetails
        {
            Title = title,
            Status = statusCode,
            Detail = message,
            Instance = context.Request.Path,
        };

        problemDetails.Extensions["traceId"] = context.TraceIdentifier;

        var response = JsonSerializer.Serialize(problemDetails);
        await context.Response.WriteAsync(response);
    }
}