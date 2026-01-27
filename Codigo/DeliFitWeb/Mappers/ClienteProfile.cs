using AutoMapper;
using Core;
using Core.DTO;
using DeliFitWeb.Models;

namespace DeliFitWeb.Mappers;

public class ClienteProfile : Profile
{
    public ClienteProfile()
    {
        CreateMap<ClienteDTO, ClienteViewModel>().ReverseMap();
        CreateMap<Cliente, ClienteViewModel>().ReverseMap();
    }
}
