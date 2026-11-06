namespace Shop.Application.Products.Queries;

public record GetProductsByIdsQuery(IReadOnlyCollection<Guid> Ids);
