using Shop.Application.Exceptions;
using Shop.Application.Interfaces;
using Shop.Application.Order.Commands;

namespace Shop.Application.Order.Handlers;

public class CancelOrderHandler
{
    private readonly IOrderRepository _orderRepo;
    private readonly IProductRepository _productRepo;
    private readonly IUnitOfWork _unitOfWork;

    public CancelOrderHandler(IOrderRepository orderRepo, IProductRepository productRepo, IUnitOfWork unitOfWork)
    {
        _orderRepo = orderRepo;
        _productRepo = productRepo;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(CancelOrderCommand command, Guid userId, CancellationToken cancellationToken)
    {
        var orderId = command.OrderId;
        var order = await _orderRepo.GetAsync(orderId, cancellationToken);
        if (order is null)
        {
            throw new NotFoundException("No order was found.");
        }

        if (order.UserId != userId)
            throw new ForbiddenException("Not allowed.");

        var wasSubmitted = order.Status == OrderStatus.Submitted;

        order.Cancel(); // only when current status is draft or submitted, otherwise throw exception

        if (wasSubmitted)
        {
            var productIds = order.Items.Select(i => i.ProductId).Distinct().ToList();
            var products = await _productRepo.GetByIdsAsync(productIds, cancellationToken);
            var productDict = products.ToDictionary(p => p.Id);

            foreach (var item in order.Items)
            {
                if (!productDict.TryGetValue(item.ProductId, out var product))
                    throw new NotFoundException("Product not found.");

                product.IncreaseStock(item.Quantity);
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
