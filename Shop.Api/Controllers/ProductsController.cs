using Microsoft.AspNetCore.Mvc;
using Shop.Application.Products.Commands;
using Shop.Application.Products.Handlers;
using Shop.Api.Contracts;
using Shop.Application.Products.Queries;
using Shop.Api.Mappers;
using Shop.Application.Common.Pagination;
using Microsoft.AspNetCore.Authorization;
using Shop.Domain.Enums;

namespace Shop.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly CreateProductHandler _createHandler;
    private readonly UpdateProductHandler _updateHandler;
    private readonly DeleteProductHandler _deleteHandler;
    private readonly GetProductHandler _getHandler;
    private readonly GetProductListHandler _getListHandler;

    private readonly GetProductsByIdsHandler _getProductsByIdsHandler;

    public ProductsController(CreateProductHandler createHandler,
        UpdateProductHandler updateHandler, 
        DeleteProductHandler deleteHandler,
        GetProductHandler getHandler,
        GetProductListHandler getListHandler,
        GetProductsByIdsHandler getProductsByIdsHandler)
    {
        _createHandler = createHandler;
        _updateHandler = updateHandler;
        _deleteHandler = deleteHandler;
        _getHandler = getHandler;
        _getListHandler = getListHandler;
        _getProductsByIdsHandler = getProductsByIdsHandler;
    }

    [Authorize(Roles = nameof(UserRole.Admin))]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody]CreateProductCommand command, CancellationToken cancellationToken)
    {
        var id = await _createHandler.Handle(command, cancellationToken);

        return Created($"/api/products/{id}", id);
    }

    [Authorize(Roles = nameof(UserRole.Admin))]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromRoute]Guid id, [FromBody] UpdateProductRequest request, CancellationToken cancellationToken)
    {
        //id는 route에서 가져오고, 나머지 필드는 request에서 가져와서 UpdateProductCommand를 생성합니다.
        var command = new UpdateProductCommand(id, request.Name, request.Price, request.Currency, request.Stock, request.Description);
        await _updateHandler.Handle(command, cancellationToken);

        return NoContent();
    }

    [Authorize(Roles = nameof(UserRole.Admin))]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute]Guid id, CancellationToken cancellationToken)
    {
        await _deleteHandler.Handle(id, cancellationToken);

        return NoContent();
    }

    [AllowAnonymous]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetAsync([FromRoute]Guid id, CancellationToken cancellationToken)
    {
        var product = await _getHandler.Handle(new GetProductQuery(id), cancellationToken);

        var response = product.ToResponse();
        return Ok(response);
    }  

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetAllAsync(CancellationToken cancellationToken, [FromQuery]int page = 1, [FromQuery] int pageSize = 20)
    {
        var products = await _getListHandler.Handle(new GetProductListQuery(new PageRequest(page, pageSize)), cancellationToken);
        var response = products.Items.Select(product => ProductMapper.ToResponse(product)).ToList();
        return Ok(new PaginatedResponse<ProductResponse>(response, products.Page, products.PageSize, products.TotalCount));
    }

    [AllowAnonymous]
    [HttpGet("bulk")]
    public async Task<IActionResult> GetBulkAsync([FromQuery] string ids, CancellationToken cancellationToken)
    {
        var idArray = ids.Split(",");
        var products = await _getProductsByIdsHandler.Handle(new GetProductsByIdsQuery(idArray.Select(i => Guid.Parse(i)).ToList()), cancellationToken);
        return Ok(products.Select(p => ProductMapper.ToResponse(p)).ToList());
        //그래서 API가 Domain Entity를 외부에 직접 노출하지 않는 구조가 유지돼.
    }
}