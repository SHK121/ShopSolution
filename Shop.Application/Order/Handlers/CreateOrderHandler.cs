using Shop.Application.Exceptions;
using Shop.Application.Interfaces;
using Shop.Application.Order.Commands;
using Shop.Domain.Entities;
using OrderEntity = Shop.Domain.Entities.Order;

namespace Shop.Application.Order.Handlers;

public class CreateOrderHandler
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateOrderHandler(IOrderRepository orderRepository, IProductRepository productRepository, IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateOrderCommand command, Guid userId, CancellationToken cancellationToken)
    {
        var order = OrderEntity.Create(userId);
        var productIds = command.Items.Select(i => i.ProductId).Distinct().ToList();
        var products = await _productRepository.GetByIdsAsync(productIds, cancellationToken);
        var productDictionary = products.ToDictionary(p => p.Id);

        foreach (var i in command.Items)
        {
            if (!productDictionary.TryGetValue(i.ProductId, out var product))
            {
                throw new NotFoundException(
                    $"Product {i.ProductId} was not found.");
            }
            var orderItem = OrderItem.Create(order.Id, product.Id, product.Name, product.Price, i.Quantity);
            order.AddItem(orderItem);
        }

        await _orderRepository.AddAsync(order, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return order.Id;
    }
}