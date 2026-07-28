using AutoMapper;
using Core.DTO;
using DeliFitWeb.Models;

namespace DeliFitWeb.Mappers;

public class FaturamentoProfile : Profile
{
    public FaturamentoProfile()
    {
        CreateMap<FaturamentoDTO, FaturamentoViewModel>().ReverseMap();
    }
}
