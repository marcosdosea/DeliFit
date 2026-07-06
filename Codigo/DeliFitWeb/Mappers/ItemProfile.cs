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
                .ReverseMap()
                .ForMember(dest => dest.Foto, opt => opt.Ignore());
        }
    }
}