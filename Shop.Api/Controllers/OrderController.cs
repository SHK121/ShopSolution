using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shop.Application.Exceptions;
using Shop.Application.Order.Commands;
using Shop.Api.Contracts;
using Shop.Application.Order.Handlers;
using Shop.Application.Order.Queries;
using System.Security.Claims;
using Shop.Api.Mappers;
using Shop.Application.Common.Pagination;

namespace Shop.Api.Controllers;

[ApiController]
[Route("api/orders")]
public class OrderController : ControllerBase
{
    private readonly CreateOrderHandler _createHandler;
    private readonly SubmitOrderHandler _submitHandler;
    private readonly PayOrderHandler _payHandler;
    private readonly GetOrderHandler _getHandler;

    private readonly GetOrderListHandler _getListHandler;

    private readonly CancelOrderHandler _cancelHandler;

    private readonly ShipOrderHandler _shipHandler;

    public OrderController(CreateOrderHandler createHandler, 
        SubmitOrderHandler submitHandler, 
        PayOrderHandler payHandler, 
        GetOrderHandler getHandler, 
        CancelOrderHandler cancelHandler,
        GetOrderListHandler getListHandler,
        ShipOrderHandler shipHandler)
    {
        _createHandler = createHandler;
        _submitHandler = submitHandler;
        _payHandler = payHandler;
        _getHandler = getHandler;
        _cancelHandler = cancelHandler;
        _getListHandler = getListHandler;
        _shipHandler = shipHandler;
    }

    private Guid GetCurrentUserId()
    {
        /*따라서 이 GetCurrentUserId()의 UnauthorizedException은 사실상 "인증은 통과했는데 Claim이 이상하다" 같은 비정상적인 상황을 방어하는 역할이라고 보면 돼.*/
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userId, out var userGuid))
            throw new AuthorizationException("Invalid user identity.");

        return userGuid;
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody]CreateOrderCommand command, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();

        var orderId = await _createHandler.Handle(command, userId, cancellationToken);
        return Created($"/api/orders/{orderId}", orderId);
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAllAsync(CancellationToken cancellationToken, [FromQuery]int page = 1, [FromQuery]int pageSize = 20)
    {
        var userId = GetCurrentUserId();

        var result = await _getListHandler.Handle(new GetOrderListQuery(userId, new PageRequest(page, pageSize)), cancellationToken);

        return Ok(new PaginatedResponse<OrderResponse>(result.Items.Select(o => OrderMapper.ToResponse(o)).ToList(), 
            result.Page, 
            result.PageSize, 
            result.TotalCount));
    }

    [Authorize]
    [HttpPost("{orderId}/submit")]
    public async Task<IActionResult> Submit([FromRoute]Guid orderId, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();

        await _submitHandler.Handle(new SubmitOrderCommand(orderId), userId, cancellationToken);

        return NoContent();
    }

    [Authorize]
    [HttpPost("{orderId}/pay")]
    public async Task<IActionResult> Pay([FromRoute] Guid orderId, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();

        await _payHandler.Handle(new PayOrderCommand(orderId), userId, cancellationToken);

        return NoContent();
    }

    [Authorize]
    [HttpPost("{orderId}/cancel")]
    public async Task<IActionResult> Cancel([FromRoute] Guid orderId, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();

        await _cancelHandler.Handle(new CancelOrderCommand(orderId), userId, cancellationToken);

        return NoContent();
    }

    [Authorize]
    [HttpPost("{orderId}/ship")]
    public async Task<IActionResult> Ship([FromRoute] Guid orderId, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();

        await _shipHandler.Handle(new ShipOrderCommand(orderId), userId, cancellationToken);

        return NoContent();
    }

    [Authorize]
    [HttpGet("{orderId}")]
    public async Task<ActionResult<OrderResponse>> Get([FromRoute]Guid orderId, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();

        var order = await _getHandler.Handle(new GetOrderQuery(orderId), userId, cancellationToken);

        return OrderMapper.ToResponse(order);
    }

    /*[Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        return Ok(User.Claims.Select(c => new
        {
            c.Type,
            c.Value
        }));
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("admin-test")]
    public IActionResult AdminTest()
    {
        return Ok("You are an admin.");
    }*/
}
