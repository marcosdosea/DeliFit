using AutoMapper;
using Core;
using Core.DTO;
using DeliFitWeb.Models;

namespace DeliFitWeb.Mappers
{
    public class RestauranteProfile : Profile
    {
        public RestauranteProfile()
        {
            CreateMap<Restaurante, RestauranteViewModel>()
                .ForMember(dest => dest.FotoFile, opt => opt.Ignore())
                .ReverseMap()
                .ForMember(dest => dest.Foto, opt => opt.Ignore());

            CreateMap<RestauranteViewModel, RestauranteDTO>().ReverseMap();
        }
    }
}