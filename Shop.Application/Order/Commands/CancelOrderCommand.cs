namespace Shop.Application.Order.Commands;

public record CancelOrderCommand(Guid OrderId);