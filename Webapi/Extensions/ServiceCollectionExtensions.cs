using Backend.Dtos.ChatDtos;
using Common.Constants;
using Data.DbContext;
using Data.Entities.User;
using Data.IRepository;
using Data.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Service.IServices;
using Service.Services;
using Service.Utility;
using Webapi.Configurations;
using Webapi.Controllers;
using Webapi.Initializer;

namespace Webapi.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection DatabaseService(this IServiceCollection services, IConfiguration configuration)
    {
        //database
        services.AddDbContext<ApplicationDbContext>(options =>
            options
                .UseSqlServer(configuration.GetConnectionString("ConnectionContext")));

        return services;
    }

    public static IServiceCollection AddService(this IServiceCollection service)
    {
        // repository
        service.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        // service
        service.AddTransient<ISendMailService, SendMailService>();
        service.AddScoped<IUserService, UserService>();
        service.AddScoped<IJwtUtils, JwtUtils>();
        service.AddScoped<IImageService, ImageService>();
        service.AddScoped<IBookService, BookService>();
        service.AddScoped<IStallService, StallService>(); 
        service.AddScoped<IPostService, PostService>(); 
        service.AddScoped<IAddressService, AddressService>(); 
        service.AddScoped<ICartService, CartService>(); 
        service.AddScoped<IOrderService, OrderService>(); 
        service.AddScoped<IOrderDetailService, OrderDetailService>(); 
        service.AddScoped<ICategoryService, CategoryService>(); 
        service.AddScoped<ICommentService, CommentService>(); 
        service.AddScoped<IChatService, ChatService>();
      
            return service;
    }

    public static IServiceCollection AddAutoMapper(this IServiceCollection service)
    {
        //auto mapper config
        var mapper = MappingConfig.RegisterMaps().CreateMapper();
        service.AddSingleton(mapper);
        service.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        return service;
    }
    
    public static IServiceCollection AddChatService(this IServiceCollection service)
    {
        service.AddSingleton<IDictionary<string, UserConnection>>(opts => new Dictionary<string, UserConnection>());
        service.AddSingleton<IDictionary<string, string>>(opts => new Dictionary<string, string>());
        return service;
    }

    public static IServiceCollection AddCustomCors(this IServiceCollection service)
    {
        service.AddCors(options =>
        {
            options.AddPolicy("Policy",
                builder =>
                {   
                    builder
                        .WithOrigins(AppSettings.CORS)
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                });
        });

        return service;
    }

    public static IServiceCollection AddSwagger(this IServiceCollection service)
    {
        service.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Web api Final project", Version = "v1" });

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description =
                    "JWT Authorization header using the Bearer scheme. \r\n\r\n " +
                    "Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\n" +
                    "Example: \"Bearer 12345abcdef\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement()
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        },
                        Scheme = "oauth2",
                        Name = "Bearer",
                        In = ParameterLocation.Header,
                    },
                    new List<string>()
                }
            });
        });

        return service;
    }

    public static IServiceCollection BackgroundService(this IServiceCollection services)
    {
        // background service

        return services;
    }

    public static IServiceCollection MailSenderService(this IServiceCollection services, IConfiguration configuration)
    {
        // email setting
        services.AddOptions();
        var mailSettings = configuration.GetSection("MailSettings");
        services.Configure<MailSettings>(mailSettings);

        return services;
    }
}