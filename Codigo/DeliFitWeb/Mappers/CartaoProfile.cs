using AutoMapper;
using Core;
using Core.DTO;
using DeliFitWeb.Models;

namespace DeliFitWeb.Mappers;

public class CartaoProfile : Profile
{
    public CartaoProfile()
    {
        CreateMap<Cartao, CartaoViewModel>().ReverseMap();
    }
}
