using AutoMapper;
using Data.Entities;
using Data.Entities.BaseEntity;
using Data.Entities.User;
using Data.ViewModels;
using Data.ViewModels.Address;
using Data.ViewModels.Book;
using Data.ViewModels.Cart;
using Data.ViewModels.Comment;
using Data.ViewModels.Post;
using Data.ViewModels.Stall;
using Data.ViewModels.User;

namespace Webapi.Configurations;

public static class MappingConfig
{
    public static MapperConfiguration RegisterMaps()
    {
        var mappingConfig = new MapperConfiguration(config =>
        {
            
            config.CreateMap<CreateRequest, User>().ReverseMap();
            
            config.CreateMap<RegisterRequest, User>().ReverseMap();
            
            config.CreateMap<User, AccountResponse>().ReverseMap();
            
            config.CreateMap<User, AuthenticateResponse>().ReverseMap();
            
            config.CreateMap<RegisterRequest, User>().ReverseMap();
            
            config.CreateMap<UpdateRequest, User>().ReverseMap();
            
            config.CreateMap<CreateBookRequest, Book>().ReverseMap();
            config.CreateMap<StallRegisterRequest, Stall>().ReverseMap(); 
            config.CreateMap<UpdateStallRequest, Stall>().ReverseMap(); 
            
            config.CreateMap<PostRegisterRequest, Post>().ReverseMap(); 
            config.CreateMap<CreateBookPostRequset, Post>().ReverseMap(); 
            config.CreateMap<CreateBookPostRequset, Book>().ReverseMap(); 
            config.CreateMap<CreateAddressRequest, Address>().ReverseMap(); 
            config.CreateMap<UpdateCartRequest, Cart>().ReverseMap(); 
            config.CreateMap<CreateCommentRequest, Comment>().ReverseMap(); 
            config.CreateMap<UpdateBookPostRequest, Book>().ReverseMap(); 
            config.CreateMap<UpdateBookPostRequest, Post>().ReverseMap(); 




            
            // config.CreateMap<Stall, StallsResponse>()
            //     .ForMember(des => des.FullName, 
            //         option => option.MapFrom(source => source.User.FullName)); 
            config.CreateMap<Stall, StallsResponse>()
                .ForMember(des => des.FullName, option => option.MapFrom(source => source.User.FullName))
                .ForMember(des => des.PhoneNumber, option => option.MapFrom(source => source.User.PhoneNumber ?? ""));

           
            config.CreateMap<UpdateRequest, User>()
                .ForAllMembers(x => x.Condition(
                    (src, dest, prop) =>
                    {
                        // ignore null & empty string properties
                        if (prop == null) return false;
                        if (prop.GetType() == typeof(string) && string.IsNullOrEmpty((string)prop)) return false;
            
                        // ignore null role
                        if (x.DestinationMember.Name == "Role") return false;
            
                        return true;
                    }
                ));
        });
        return mappingConfig;
    }
}
