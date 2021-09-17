using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using API.interfaces;
using API.Data;
using API.Services;
using Microsoft.EntityFrameworkCore;


namespace API.Extensions
{
    public static class ApplicationServiceExtensions
    {
        
            public static IServiceCollection AddApplicationServices(this IServiceCollection i_Services, IConfiguration i_Config)
            {

                 i_Services.AddScoped<ITokenService, TokenService>();
                 i_Services.AddDbContext<DataContext>(options => 
                 {

                    options.UseSqlite(i_Config.GetConnectionString("DefaultConnection"));

                });

                return i_Services;

            }

    }
}