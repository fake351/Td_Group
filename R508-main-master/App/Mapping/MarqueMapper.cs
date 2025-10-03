using App.Models;
using AutoMapper;
using App.DTO;

namespace App.Mapping
{
    public class MarqueMapper : Profile
    {
        public MarqueMapper() 
        {
            CreateMap<Marque, MarqueDto>()
                .ForMember(dest => dest.NomMarque, opt => opt.MapFrom(src => src.NomMarque));

            CreateMap<MarqueDto, Marque>()
                .ForMember(dest => dest.NomMarque, opt => opt.MapFrom(src => src.NomMarque));
        }
    }
}
