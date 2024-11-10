using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using Contracts;

namespace AuctionService.RequestHelpers;

public class MappingProfiles: Profile
{
    public MappingProfiles()
    {
        CreateMap<Auction, AuctionDto>().IncludeMembers(c => c.Item);
        CreateMap<Item, AuctionDto>();
        CreateMap<CreateAuctionDto, Auction>()
            .ForMember(d => d.Item, 
                o => o.MapFrom(s => s));
        CreateMap<CreateAuctionDto, Item>();
        
        CreateMap<AuctionDto, AuctionCreated>();
        CreateMap<Auction, AuctionUpdated>().IncludeMembers(c => c.Item);
        CreateMap<Item, AuctionUpdated>();
    }
}