using AutoMapper;
using Core;
using DeliFitWeb.Models;

namespace DeliFitWeb.Mappers
    

{
    public class ClienteProfile : Profile
    {
        public ClienteProfile()
        {
            CreateMap<ClienteModel,Cliente>().ReverseMap();
        }
    }
}
