using CashFlow.Ledger.Api.Mappings;
using CashFlow.Ledger.Api.Middlewares;
using CashFlow.Ledger.Api.Requests;
using CashFlow.Ledger.Api.Responses;
using CashFlow.Ledger.Application.LedgerEntries.Queries.List;
using Mediator;
using Microsoft.AspNetCore.Mvc;

namespace CashFlow.Ledger.Api.Controllers;

[ApiController]
[Route("v1/lancamentos")]
public sealed class LedgerEntriesController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<LedgerEntryResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<LedgerEntryResponse>> Create(
        [FromBody] CreateLedgerEntryRequest request,
        CancellationToken cancellationToken)
    {
        var idempotencyKey = HttpContext.GetIdempotencyKey();
        var command = request.ToCommand(idempotencyKey);
        var result = await mediator.Send(command, cancellationToken);
        var response = result.ToResponse();

        return Created($"/v1/lancamentos/{response.Id}", response);
    }

    [HttpGet]
    [ProducesResponseType<IReadOnlyList<LedgerEntryListItemResponse>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<LedgerEntryListItemResponse>>> List(
        [FromQuery(Name = "comercianteId")] string merchantId,
        CancellationToken cancellationToken)
    {
        var entries = await mediator.Send(
            new ListLedgerEntriesQuery(merchantId),
            cancellationToken);

        return Ok(entries.Select(entry => entry.ToListItemResponse()).ToList());
    }
}
