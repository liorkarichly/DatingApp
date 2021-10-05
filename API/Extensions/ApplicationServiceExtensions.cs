using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using API.interfaces;
using API.Data;
using API.Services;
using Microsoft.EntityFrameworkCore;
using API.Helpers;

namespace API.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection i_Services, IConfiguration i_Config)
        {

            //Operation only in scope
            i_Services.AddScoped<ITokenService, TokenService>();
            i_Services.AddScoped<IUserRepository, UserRepository>();//Add service of user repository, and make to access to use it
            i_Services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);//WE neet to tell about the other project by the Assembly
            i_Services.AddDbContext<DataContext>(options => 
            {

                options.UseSqlite(i_Config.GetConnectionString("DefaultConnection"));

            });

            return i_Services;

        }

    }

}