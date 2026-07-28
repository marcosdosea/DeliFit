using AutoMapper;
using Core;
using Core.DTO;
using DeliFitAPI.Models;

namespace DeliFitAPI.Mappers;

public class ClienteProfile : Profile
{
    public ClienteProfile()
    {
        CreateMap<ClienteDTO, ClienteViewModel>().ReverseMap();
        CreateMap<Cliente, ClienteViewModel>().ReverseMap();
    }
}
