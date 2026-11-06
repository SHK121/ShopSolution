using Shop.Api.Contracts;
using Shop.Application.Exceptions;
using Shop.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Shop.Api.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        } catch (Exception ex)
        {
            ErrorResponse response;
            switch (ex)
            {
                case AuthorizationException:
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    response = new ErrorResponse(
                        "UNAUTHORIZED_ERROR",
                        ex.Message, context.TraceIdentifier);
                    break;
                case ForbiddenException:
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    response = new ErrorResponse(
                        "FORBIDDEN_ERROR",
                        ex.Message, context.TraceIdentifier);
                    break;
                case ValidationException:
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    response = new ErrorResponse(
                        "VALIDATION_ERROR",
                        ex.Message, context.TraceIdentifier);
                    break;

                case NotFoundException:
                    context.Response.StatusCode = StatusCodes.Status404NotFound;
                    response = new ErrorResponse(
                        "NOT_FOUND",
                        ex.Message, context.TraceIdentifier);
                    break;
                case DbUpdateConcurrencyException:
                    context.Response.StatusCode = StatusCodes.Status409Conflict;
                    response = new ErrorResponse(
                        "CONCURRENCY_ERROR",
                        "The resource was modified by another request. Please try again.", context.TraceIdentifier);
                    break;
                default:
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    response = new ErrorResponse(
                        "INTERNAL_SERVER_ERROR",
                        "An unexpected error occurred.", context.TraceIdentifier);
                    break;
            }

            await context.Response.WriteAsJsonAsync(response);
        }
    }
}