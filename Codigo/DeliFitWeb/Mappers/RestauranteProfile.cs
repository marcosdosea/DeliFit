using AutoMapper;
using Core;
using DeliFitWeb.Models;
namespace DeliFitWeb.Mappers
{
    public class RestauranteProfile : Profile
    {
        public RestauranteProfile()
        {
            CreateMap<RestauranteModel, Restaurante>().ReverseMap();
        }

    }
}
