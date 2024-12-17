using System.Net;
using System.Net.Http.Json;
using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.IntegrationTests.Fixtures;
using AuctionService.IntegrationTests.Util;
using AuctionService.UnitTests.Utils;
using Contracts;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace AuctionService.IntegrationTests;

[Collection("Shared collection")]
public class AuctionBusTests(CustomWebAppFactory factory) : IAsyncLifetime
{
    private readonly HttpClient _httpClient = factory.CreateClient();
    private readonly ITestHarness _testHarness = factory.Services.GetTestHarness();

    [Fact]
    public async Task CreateAuction_withValidObject_ShouldPublishAuctionCreated()
    {
        var auction = GetAuctionForCreate();
        _httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForUser("bob"));

        var response = await _httpClient.PostAsJsonAsync($"api/auctions", auction);

        response.EnsureSuccessStatusCode();
        Assert.True(await _testHarness.Published.Any<AuctionCreated>());
        
    }
    
    public Task InitializeAsync() => Task.CompletedTask;
    

    public Task DisposeAsync()
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AuctionDbContext>();
        DbHelper.ReinitDbForTests(db);
        return Task.CompletedTask;
    }
    
    private CreateAuctionDto GetAuctionForCreate()
    {
        return new CreateAuctionDto
        {
            Make = "Test",
            Model = "testModel",
            Year = 0,
            Color = "test",
            Mileage = 0,
            ImageUrl = "test",
            ReservePrice = 0,
            AuctionEnd = default
        };
    }
}