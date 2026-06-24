using CashFlow.Ledger.Api.ExceptionHandling;
using CashFlow.Ledger.Infrastructure.DependencyInjection;
using CashFlow.Ledger.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ApiExceptionHandler>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks().AddDbContextCheck<LedgerDbContext>();
builder.Services.AddLedger(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler();
app.UseMiddleware<CashFlow.Ledger.Api.Middlewares.CorrelationIdMiddleware>();
app.UseMiddleware<CashFlow.Ledger.Api.Middlewares.IdempotencyKeyMiddleware>();
app.MapControllers();
app.MapHealthChecks("/health");

app.Run();

public partial class Program;
