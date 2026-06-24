using CashFlow.Balance.Api.Mappings;
using CashFlow.Balance.Api.Responses;
using CashFlow.Balance.Application.DailyBalances.Queries.Get;
using Mediator;
using Microsoft.AspNetCore.Mvc;

namespace CashFlow.Balance.Api.Controllers;

[ApiController]
[Route("v1/saldo")]
public sealed class DailyBalancesController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<DailyBalanceResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DailyBalanceResponse>> Get(
        [FromQuery(Name = "comercianteId")] string merchantId,
        [FromQuery(Name = "data")] DateOnly? businessDate,
        CancellationToken cancellationToken)
    {
        var date = businessDate ?? DateOnly.FromDateTime(DateTime.UtcNow);

        var result = await mediator.Send(
            new GetDailyBalanceQuery(merchantId, date),
            cancellationToken);

        if (result is null)
            return NotFound();

        return Ok(result.ToResponse());
    }
}
