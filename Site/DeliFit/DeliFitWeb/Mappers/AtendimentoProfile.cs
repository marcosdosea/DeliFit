using AutoMapper;
using Core;
using DeliFitWeb.Models;

namespace DeliFitWeb.Mappers;

public class AtendimentoProfile : Profile
{
    public AtendimentoProfile()
    {
        CreateMap<AtendimentoViewModel, Atendimento>().ReverseMap();
    }
}
