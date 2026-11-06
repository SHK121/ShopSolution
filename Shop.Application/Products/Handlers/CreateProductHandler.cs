using Shop.Application.Interfaces;
using Shop.Domain.Entities;
using Shop.Application.Products.Commands;
using Shop.Domain.ValueObjects;

namespace Shop.Application.Products.Handlers;

public class CreateProductHandler {
    private readonly IProductRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateProductHandler(IProductRepository repository, IUnitOfWork unitOfWork) {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateProductCommand command, CancellationToken cancellationToken){
        var money = Money.Create(command.Price, command.Currency);

        var product = Product.Create(
            command.Name,
            money,
            command.Stock,
            command.Description);

        await _repository.AddAsync(product);

        await _unitOfWork.SaveChangesAsync();

        return product.Id;
    }
}