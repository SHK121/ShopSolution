namespace Shop.Application.Order.Commands;

public record CreateOrderItemCommand(Guid ProductId, int Quantity)
{
}
