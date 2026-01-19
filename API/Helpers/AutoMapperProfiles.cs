using System;
using System.Linq;
using API.DTOs;
using API.Entities;
using API.Extensions;
using AutoMapper;

namespace API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<AppUser, MemberDto>()
                .ForMember(dest => dest.PhotoUrl, opt => opt.MapFrom(src => 
                    src.Photos.FirstOrDefault(x => x.IsMain).Url))
                .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.DateOfBirth.CalculateAge()));
            CreateMap<Photo, PhotoDto>();
            CreateMap<MemberUpdateDto, AppUser>();
            CreateMap<RegisterDto, AppUser>()
				.ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Username.ToLower()))
				.ForMember(dest => dest.KnownAs, opt => opt.MapFrom(src => src.KnownAs))
				.ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender))
				.ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.DateOfBirth))
				.ForMember(dest => dest.City, opt => opt.MapFrom(src => src.City))
				.ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.Country))
				.ForMember(dest => dest.Created, opt => opt.MapFrom(src => DateTime.UtcNow))
				.ForMember(dest => dest.LastActive, opt => opt.MapFrom(src => DateTime.UtcNow))
				.ForMember(dest => dest.Tier, opt => opt.MapFrom(src => "Free"));
            CreateMap<AppUser, UserDto>()
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.Tier, opt => opt.MapFrom(src => src.Tier));
				//.ForMember(dest => dest.IsSubscriptionActive, opt => opt.MapFrom(src => src.IsSubscriptionActive))
				//.ForMember(dest => dest.IsTwoFactorEnabled, opt => opt.MapFrom(src => src.IsTwoFactorEnabled));
			CreateMap<Message, MessageDto>()
                .ForMember(dest => dest.SenderPhotoUrl, opt => opt.MapFrom(src => 
                    src.Sender.Photos.FirstOrDefault(x => x.IsMain).Url))
                .ForMember(dest => dest.RecipientPhotoUrl, opt => opt.MapFrom(src => 
                    src.Recipient.Photos.FirstOrDefault(x => x.IsMain).Url));
            CreateMap<MessageDto, Message>();
        }
    }
}