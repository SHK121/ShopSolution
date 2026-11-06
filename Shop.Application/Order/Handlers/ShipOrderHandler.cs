using Shop.Application.Exceptions;
using Shop.Application.Interfaces;
using Shop.Application.Order.Commands;

namespace Shop.Application.Order.Handlers;

public class ShipOrderHandler
{
    private readonly IOrderRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public ShipOrderHandler(IOrderRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(ShipOrderCommand command, Guid userId, CancellationToken cancellationToken)
    {
        var orderId = command.OrderId;
        var order = await _repository.GetAsync(orderId, cancellationToken);
        if (order is null)
        {
            throw new NotFoundException("No order was found.");
        }

        if (order.UserId != userId)
            throw new ForbiddenException("Not allowed.");

        order.Ship();
        await _unitOfWork.SaveChangesAsync();
    }
}
