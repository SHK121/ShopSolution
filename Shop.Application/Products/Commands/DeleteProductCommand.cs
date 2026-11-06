using System;
using System.Collections.Generic;
using System.Text;

namespace Shop.Application.Products.Commands;

public record DeleteProductCommand
{    
    public Guid Id { get; set; }

    public DeleteProductCommand(Guid id)
    {
        Id = id;
    }
}
