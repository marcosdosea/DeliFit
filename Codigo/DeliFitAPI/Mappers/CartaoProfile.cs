using AutoMapper;
using Core;
using DeliFitAPI.Models;

namespace DeliFitAPI.Mappers;

public class CartaoProfile : Profile
{
    public CartaoProfile()
    {
        CreateMap<Cartao, CartaoViewModel>().ReverseMap();
    }
}