using AutoMapper;
using WebApp.ViewModels;
using WebAPI.DTOs;
using WebAPI.Models;


namespace CulturalHeritageWebApp.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CulturalHeritageDto, CulturalHeritageDetailsViewModel>()
    .ForMember(dest => dest.NationalMinorityName, opt => opt.MapFrom(src => src.NationalMinorityName))
    .ReverseMap();
            CreateMap<CulturalHeritageDto, CulturalHeritageViewModel>().ReverseMap();
            CreateMap<CommentDto, CommentViewModel>().ReverseMap();
            CreateMap<NationalMinorityDto, NationalMinorityViewModel>().ReverseMap();
            CreateMap<CulturalHeritageDto, CulturalHeritage>()
    .ForMember(dest => dest.NationalMinorityId, opt => opt.MapFrom(src => src.NationalMinorityID))
    .ReverseMap();
            CreateMap<CulturalHeritageViewModel, CulturalHeritageDto>()
    .ForMember(dest => dest.HeritageID, opt => opt.MapFrom(src => src.HeritageID))
    .ForMember(dest => dest.NationalMinorityID, opt => opt.MapFrom(src => src.NationalMinorityID))
    .ReverseMap();
            CreateMap<NationalMinorityDto, NationalMinorityViewModel>().ReverseMap();

            // Theme Mapping
            CreateMap<ThemeDto, ThemeViewModel>().ReverseMap();
            CreateMap<CulturalHeritageDto, CulturalHeritageDetailsViewModel>()
    .ForMember(dest => dest.NationalMinorityName, opt => opt.MapFrom(src => src.NationalMinorityName))
    .ReverseMap();
        }
    }
}
