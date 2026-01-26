using AutoMapper;
using Core.DTO;
using DeliFitWeb.Models;

namespace DeliFitWeb.Mappers;

public class ClienteProfile : Profile
{
    public ClienteProfile()
    {
        CreateMap<ClienteDTO, ClienteModel>().ReverseMap();
    }
}
