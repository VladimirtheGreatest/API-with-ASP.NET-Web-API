using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheCodeCamp.Models;

namespace TheCodeCamp.Data
{
    public class CampMappingProfile : Profile
    {
        public CampMappingProfile()
        {
            //CreateMap<Camp, CampModel>().ForMember(c => c.Venue, opt => opt.MapFrom(m => m.Location.VenueName));
            CreateMap <Speaker, SpeakerModel>().ReverseMap();
            CreateMap <Talk, TalkModel>().ReverseMap().ForMember(x => x.Speaker, opt => opt.Ignore()).ForMember(x => x.Camp, opt => opt.Ignore());
            CreateMap <Camp, CampModel>().ForMember(c => c.Venue, opt => opt.MapFrom(m => m.Location.VenueName)).ReverseMap();
        }
    }
}
