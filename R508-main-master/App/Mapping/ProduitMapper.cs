using AutoMapper;
using App.Models;
using App.DTO;

namespace App.Mapping
{
    public class ProduitMapper : Profile
    {
        public ProduitMapper()
        {
        CreateMap<Produit, ProduitDto>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.TypeProduitNavigation != null ? src.TypeProduitNavigation.NomTypeProduit : string.Empty))
            .ForMember(dest => dest.Marque, opt => opt.MapFrom(src => src.MarqueNavigation != null ? src.MarqueNavigation.NomMarque : string.Empty))
            .ReverseMap();
            CreateMap<Produit, ProduitDetailDto>().ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.TypeProduitNavigation != null ? src.TypeProduitNavigation.NomTypeProduit : string.Empty))
            .ForMember(dest => dest.Marque, opt => opt.MapFrom(src => src.MarqueNavigation != null ? src.MarqueNavigation.NomMarque : string.Empty))
            .ReverseMap();
        }
    }
}