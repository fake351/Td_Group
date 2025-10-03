using AutoMapper;
using App.DTO;
using App.Models;

namespace App.Tests.Utils
{
    public static class TestHelper
    {
        public static IMapper CreateMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Marque, MarqueDto>().ReverseMap();
                cfg.CreateMap<TypeProduit, TypeProduitDto>().ReverseMap();
                // Ajoute ici les autres mappings nécessaires
            });
            return config.CreateMapper();
        }
    }
}