using AutoMapper;
using Core;
using DeliFitWeb.Models;

namespace DeliFitWeb.Mappers;

public class EnderecoProfile : Profile
{
    public EnderecoProfile()
    {
        CreateMap<EnderecoViewModel, Endereco>().ReverseMap();
    }
}
