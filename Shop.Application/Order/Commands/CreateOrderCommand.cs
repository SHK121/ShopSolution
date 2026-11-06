namespace Shop.Application.Order.Commands;

public record CreateOrderCommand(IReadOnlyCollection<CreateOrderItemCommand> Items);