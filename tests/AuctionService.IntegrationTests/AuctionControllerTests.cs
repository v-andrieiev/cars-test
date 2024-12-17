using System.Net;
using System.Net.Http.Json;
using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.IntegrationTests.Fixtures;
using AuctionService.IntegrationTests.Util;
using AuctionService.UnitTests.Utils;
using Microsoft.Extensions.DependencyInjection;

namespace AuctionService.IntegrationTests;

[Collection("Shared collection")]
public class AuctionControllerTests(CustomWebAppFactory factory) : IAsyncLifetime
{
    private readonly HttpClient _httpClient = factory.CreateClient();
    private const string GT_ID = "afbee524-5972-4075-8800-7d1f9d7b0a0c";

    [Fact]
    public async Task GetAuctions_ShouldReturn3Auctions()
    {
        //arrange
        // act
        var response = await _httpClient.GetFromJsonAsync<List<AuctionDto>>("api/auctions");
        
        //assert
        Assert.Equal(3, response.Count);
    }
    
    [Fact]
    public async Task GetAuctionById_WithValidId_ShouldReturnAuction()
    {
        //arrange
        
        // act
        var response = await _httpClient.GetFromJsonAsync<AuctionDto>($"api/auctions/{GT_ID}");
        
        //assert
        Assert.Equal("GT", response.Model);
    }
    
    [Fact]
    public async Task GetAuctionById_WithInvalidId_ShouldReturn404NotFound()
    {
        //arrange
        
        // act
        var response = await _httpClient.GetAsync($"api/auctions/{Guid.NewGuid()}");
        
        //assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    
    [Fact]
    public async Task GetAuctionById_WithInvalidGuid_ShouldReturn400()
    {
        //arrange
        
        // act
        var response = await _httpClient.GetAsync($"api/auctions/invalid_guid");
        
        //assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    
    [Fact]
    public async Task CreateAuction_WithNoAuth_ShouldReturn401()
    {
        //arrange
        var auction = GetAuctionForCreate();
        // act
        var response = await _httpClient.PostAsJsonAsync($"api/auctions", auction);
        
        //assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
    
    [Fact]
    public async Task CreateAuction_WithAuth_ShouldReturn201()
    {
        //arrange
        var auction = GetAuctionForCreate();
        _httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForUser("bob"));
        // act
        var response = await _httpClient.PostAsJsonAsync($"api/auctions", auction);
        
        //assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var createdAuction = await response.Content.ReadFromJsonAsync<AuctionDto>();
        Assert.Equal("bob", createdAuction.Seller);
    }

    [Fact]
    public async Task CreateAuction_WithInvalidCreateAuctionDto_ShouldReturn400()
    {
        // arrange
        var auction = new 
        {
            Make = "",
        };
        _httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForUser("bob"));
        
        // act
        var response = await _httpClient.PostAsJsonAsync($"api/auctions", auction);
        
        // assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateAuction_WithValidUpdateDtoAndUser_ShouldReturn200()
    {
        // arrange
        var auction = GetUpdateAuctionDto();
        _httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForUser("bob"));
        
        // act
        var response = await _httpClient.PutAsJsonAsync($"api/auctions/{GT_ID}", auction);
        
        // assert
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task UpdateAuction_WithValidUpdateDtoAndInvalidUser_ShouldReturn403()
    {
        // arrange
        var auction = GetUpdateAuctionDto();
        _httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForUser("alice"));
        
        // act
        var response = await _httpClient.PutAsJsonAsync($"api/auctions/{GT_ID}", auction);
        
        // assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
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
    
    private UpdateAuctionDto GetUpdateAuctionDto()
    {
        return new UpdateAuctionDto
        {
            Make = "make_modified",
            Model = "model_modified",
            Year = 0,
            Color = "test",
            Mileage = 0,
        };
    }
}