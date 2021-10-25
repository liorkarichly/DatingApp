using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using API.interfaces;
using API.Data;
using API.Services;
using Microsoft.EntityFrameworkCore;
using API.Helpers;
using API.SignalR;

namespace API.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection i_Services, IConfiguration i_Config)
        {
            
            
            i_Services.AddSingleton<PresenceTracker>();
                //I turn the service to configuration for take the service of cloudinary and he need to search the section "CloudinarySettings"
            i_Services.Configure<CloudinarySettings>(i_Config.GetSection("CloudinarySettings"));

            //Operation only in scope to use in service
            i_Services.AddScoped<ITokenService, TokenService>();
            i_Services.AddScoped<IPhotoService, PhotoService>();
             i_Services.AddScoped<IUnitOfWork, UnitOfWork>();
            // i_Services.AddScoped<ILikeRepository, LikesRepository>();
            // i_Services.AddScoped<IMessageRepository, MessageRepository>();
            i_Services.AddScoped<LogUserActivity>();
            //i_Services.AddScoped<IUserRepository, UserRepository>();//Add service of user repository, and make to access to use it
            i_Services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);//WE need to tell about the other project by the Assembly
         
            i_Services.AddDbContext<DataContext>(options => 
            {

                options.UseSqlite(i_Config.GetConnectionString("DefaultConnection"));

            });

            return i_Services;

        }

    }

}