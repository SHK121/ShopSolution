namespace Shop.Api.Contracts;

public record ErrorResponse(
    string Code,
    string Message,
    string? TraceId);
