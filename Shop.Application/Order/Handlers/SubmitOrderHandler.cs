using Shop.Application.Exceptions;
using Shop.Application.Interfaces;
using Shop.Application.Order.Commands;
using Shop.Domain.Exceptions;

namespace Shop.Application.Order.Handlers;

public class SubmitOrderHandler
{
    private readonly IOrderRepository _orderRepo;
    private readonly IProductRepository _productRepo;
    private readonly IUnitOfWork _unitOfWork;

    public SubmitOrderHandler(IOrderRepository orderRepo, IProductRepository productRepo, IUnitOfWork unitOfWork)
    {
        _orderRepo = orderRepo;
        _productRepo = productRepo;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(SubmitOrderCommand command, Guid userId, CancellationToken cancellationToken)
    {
        var orderId = command.OrderId;
        var order = await _orderRepo.GetAsync(orderId, cancellationToken);
        if (order is null)
        {
            throw new NotFoundException("No order was found.");
        }

        if (order.UserId != userId)
            throw new ForbiddenException("Not allowed.");

        order.Submit();

        var productIds = order.Items.Select(i => i.ProductId).Distinct().ToList();
        var products = await _productRepo.GetByIdsAsync(productIds, cancellationToken);
        var productDict = products.ToDictionary(p => p.Id, p => p);

        foreach (var item in order.Items)
        {
            if (!productDict.TryGetValue(item.ProductId, out var product))
                throw new NotFoundException("Product not found.");

            product.DecreaseStock(item.Quantity);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
