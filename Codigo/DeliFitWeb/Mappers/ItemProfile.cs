using AutoMapper;
using Core;
using DeliFitWeb.Models;

namespace DeliFitWeb.Mappers
{
    public class ItemProfile : Profile
    {
        public ItemProfile()
        {
            CreateMap<ItemViewModel,Item>().ReverseMap();
        }
    }
}
