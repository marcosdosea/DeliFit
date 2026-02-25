using AutoMapper;
using Core;
using Core.DTO;
using DeliFitAPI.Models;

namespace DeliFitAPI.Mappers
{
    public class RestauranteProfile : Profile
    {
        public RestauranteProfile()
        {
            CreateMap<RestauranteViewModel, Restaurante>().ReverseMap();

            CreateMap<RestauranteViewModel, RestauranteDTO>().ReverseMap();
        }

    }
}
