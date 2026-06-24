using System.Net;
using System.Net.Http.Json;
using CashFlow.Ledger.Api.Responses;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace CashFlow.Ledger.IntegrationTests.Api;

public sealed class CreateLedgerEntryEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public CreateLedgerEntryEndpointTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Post_ShouldCreateLedgerEntry()
    {
        // Arrange
        using var request = new HttpRequestMessage(HttpMethod.Post, "/v1/lancamentos")
        {
            Content = JsonContent.Create(new
            {
                comercianteId = "merchant-123",
                tipo = "credito",
                valor = 150.00m,
                data = "2026-06-23",
                descricao = "Daily sale"
            })
        };
        request.Headers.Add("Idempotency-Key", Guid.NewGuid().ToString());

        // Act
        var response = await _client.SendAsync(request);
        var body = await response.Content.ReadFromJsonAsync<LedgerEntryResponse>();

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(body);
        Assert.Equal("credito", body.Type);
        Assert.Equal(150.00m, body.Amount);
        Assert.Equal(new DateOnly(2026, 6, 23), body.BusinessDate);
        Assert.Equal("Daily sale", body.Description);
    }

    [Fact]
    public async Task Post_ShouldReturnBadRequest_WhenIdempotencyKeyIsMissing()
    {
        // Arrange
        using var request = new HttpRequestMessage(HttpMethod.Post, "/v1/lancamentos")
        {
            Content = JsonContent.Create(new
            {
                comercianteId = "merchant-123",
                tipo = "credito",
                valor = 150.00m,
                data = "2026-06-23"
            })
        };

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
