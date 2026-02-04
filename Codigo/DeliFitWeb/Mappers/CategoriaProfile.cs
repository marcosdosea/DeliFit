using AutoMapper;
using Core.DTO;
using DeliFitWeb.Models;

namespace DeliFitWeb.Mappers
{
    public class CategoriaProfile : Profile
    {
        public CategoriaProfile()
        {
            CreateMap<CategoriaDTO, CategoriaViewModel>().ReverseMap();
        }
    }
}
