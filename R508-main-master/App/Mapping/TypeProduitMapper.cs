using App.DTO;
using App.Models;
using AutoMapper;

namespace App.Mapping
{
    public class TypeProduitMapper : Profile
    {
        public TypeProduitMapper()
        {
            CreateMap<TypeProduit, TypeProduitDto>()
                .ForMember(dest => dest.NomTypeProduit, opt => opt.MapFrom(src => src.NomTypeProduit));

            CreateMap<TypeProduitDto, TypeProduit>()
                .ForMember(dest => dest.NomTypeProduit, opt => opt.MapFrom(src => src.NomTypeProduit));
        }

    }
}
