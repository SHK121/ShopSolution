using Shop.Application.Interfaces;
using Shop.Application.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shop.Application.Products.Handlers;

public class DeleteProductHandler
{
    private readonly IProductRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteProductHandler(IProductRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(Guid productId, CancellationToken cancellationToken)
    {
        var product = await _repository.GetAsync(productId, cancellationToken);

        if (product == null)
        {
            throw new NotFoundException($"Product '{productId}' was not found.");
        } else
        {
            _repository.Remove(product);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
