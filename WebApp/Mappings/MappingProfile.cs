using AutoMapper;
using WebApp.ViewModels;
using WebAPI.DTOs;


namespace CulturalHeritageWebApp.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CulturalHeritageDto, CulturalHeritageViewModel>().ReverseMap();

        }
    }
}
