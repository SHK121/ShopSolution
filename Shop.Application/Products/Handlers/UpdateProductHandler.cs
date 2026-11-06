using Shop.Application.Interfaces;
using Shop.Application.Products.Commands;
using Shop.Application.Exceptions;
using Shop.Domain.ValueObjects;

namespace Shop.Application.Products.Handlers;

public class UpdateProductHandler
{
    private readonly IProductRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateProductHandler(IProductRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(UpdateProductCommand command, CancellationToken cancellationToken)
    {
        var product = await _repository.GetAsync(command.Id, cancellationToken);

        if (product == null)
        {
            throw new NotFoundException($"Product '{command.Id}' was not found.");
        }
        else
        {
            var money = Money.Create(command.Price, command.Currency);
            product.Update(command.Name, money, command.Stock, command.Description);

           // await _repository.UpdateAsync(product);

            await _unitOfWork.SaveChangesAsync();
            return;
        }
    }
}
