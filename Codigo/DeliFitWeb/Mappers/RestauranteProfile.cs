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
            CreateMap<RestauranteViewModel, Restaurante>().ReverseMap();

            CreateMap<RestauranteViewModel, RestauranteDTO>().ReverseMap();
        }

    }
}
