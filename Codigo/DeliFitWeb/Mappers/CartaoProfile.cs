using AutoMapper;
using Core;
using DeliFitWeb.Models;

namespace DeliFitWeb.Mappers;

public class CartaoProfile : Profile
{
    public CartaoProfile()
    {
        CreateMap<Cartao, CartaoViewModel>().ReverseMap();
    }
}
