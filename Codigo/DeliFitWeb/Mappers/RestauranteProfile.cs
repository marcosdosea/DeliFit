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
            CreateMap<RestauranteModel, Restaurante>().ReverseMap();

            CreateMap<RestauranteModel, RestauranteDTO>().ReverseMap();
        }

    }
}
