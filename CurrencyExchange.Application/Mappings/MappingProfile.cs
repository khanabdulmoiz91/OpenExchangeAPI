using AutoMapper;
using CurrencyExchange.Application.DTOs;
using CurrencyExchange.Core.Entities;

namespace CurrencyExchange.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ConversionHistory, ConvertCurrencyResponse>()
                .ForMember(dest => dest.OriginalAmount, opt => opt.MapFrom(src => src.Amount));
                
            CreateMap<ConversionHistory, ConversionHistoryDto>();
            
            CreateMap<ConversionHistoryDto, ConversionHistory>();
        }
    }
}