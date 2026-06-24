using CashFlow.Balance.Infrastructure.DependencyInjection;
using CashFlow.Balance.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks().AddDbContextCheck<BalanceDbContext>();
builder.Services.AddBalance(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<CashFlow.Balance.Api.Middlewares.CorrelationIdMiddleware>();
app.MapControllers();
app.MapHealthChecks("/health");

app.Run();

public partial class Program;
