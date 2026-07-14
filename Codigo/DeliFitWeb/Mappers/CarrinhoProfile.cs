using AutoMapper;
using Core;
using DeliFitWeb.Models;

namespace DeliFitWeb.Mappers;

public class CarrinhoProfile : Profile
{
    public CarrinhoProfile()
    {
        CreateMap<Carrinho, CarrinhoViewModel>().ReverseMap();
    }
}
