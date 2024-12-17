using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Contracts;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Controllers;

[ApiController]
[Route("api/auctions")]
public class AuctionController(
        IAuctionRepository repo, 
        IMapper mapper, 
        IPublishEndpoint publishEndpoint): ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<AuctionDto>>> GetALlAuctions(string date)
    {
        return await repo.GetAuctionsAsync(date);
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<AuctionDto>> GetAuctionId(Guid id)
    {
        var auction = await repo.GetAuctionByIdAsync(id);

        if (auction == null) return NotFound();
        
        return auction;
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<AuctionDto>> CreateAuction(CreateAuctionDto auctionDto)
    {
        var auction = mapper.Map<Auction>(auctionDto);
        auction.Seller = User.Identity.Name;

        repo.AddAuction(auction);
        
        var newAuction = mapper.Map<AuctionDto>(auction);

        await publishEndpoint.Publish<AuctionCreated>(mapper.Map<AuctionCreated>(newAuction));

        var result = await repo.SaveChangesAsync();
        
        if (!result) return BadRequest("Could not save changes to DB");

        return CreatedAtAction(nameof(GetAuctionId),
            new {auction.Id}, newAuction);
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateAuction(Guid id, UpdateAuctionDto updateAuctionDto)
    {
        var auction = await repo.GetAuctionEntityById(id);

        if (auction == null) return NotFound();

        if (auction.Seller != User.Identity.Name) return Forbid();
        
        auction.Item.Make = updateAuctionDto.Make ?? auction.Item.Make;
        auction.Item.Model = updateAuctionDto.Model ?? auction.Item.Model;
        auction.Item.Color = updateAuctionDto.Color ?? auction.Item.Color;
        auction.Item.Mileage = updateAuctionDto.Mileage ?? auction.Item.Mileage;
        auction.Item.Year = updateAuctionDto.Year ?? auction.Item.Year;
        
        var upd = mapper.Map<AuctionUpdated>(auction);
        await publishEndpoint.Publish<AuctionUpdated>(mapper.Map<AuctionUpdated>(auction));
        
        var result = await repo.SaveChangesAsync();
       
        if (!result) return BadRequest("Problem saving changes");

        return Ok();
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAuction(Guid id)
    {
        var auction = await repo.GetAuctionEntityById(id);

        if (auction == null) return NotFound();
        
        if (auction.Seller != User.Identity.Name) return Forbid();
        
        await publishEndpoint.Publish<AuctionDeleted>(new AuctionDeleted
        {
            Id = id.ToString()
        });
        
        repo.RemoveAuction(auction);
        
        var result = await repo.SaveChangesAsync();

        if (!result) return BadRequest("Could delete.");

        return Ok();
    }
}