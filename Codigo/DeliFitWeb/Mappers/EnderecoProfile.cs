using AutoMapper;
using Core;
using DeliFitWeb.Models;

namespace DeliFitWeb.Mappers
{
    public class EnderecoProfile : Profile
    {
        public EnderecoProfile()
        {
            CreateMap<EnderecoModel, Endereco>().ReverseMap();
        }
    }
}
