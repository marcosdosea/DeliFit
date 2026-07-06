using AutoMapper;
using Core;
using DeliFitWeb.Models;

namespace DeliFitWeb.Mappers
{
    public class ItemProfile : Profile
    {
        public ItemProfile()
        {
            CreateMap<Item, ItemViewModel>()
                .ForMember(dest => dest.FotoFile, opt => opt.Ignore())
                .ForMember(dest => dest.CategoriaIds, opt => opt.MapFrom(src => src.Categorias.Select(c => c.Id)))
                .ForMember(dest => dest.CategoriaNomes, opt => opt.MapFrom(src => src.Categorias.Select(c => c.Nome)))
                .ReverseMap()
                .ForMember(dest => dest.Foto, opt => opt.Ignore())
                .ForMember(dest => dest.Categorias, opt => opt.Ignore());
        }
    }
}
