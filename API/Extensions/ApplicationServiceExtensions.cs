using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using API.interfaces;
using API.Data;
using API.Services;
using Microsoft.EntityFrameworkCore;
using API.Helpers;
using API.SignalR;
using System;

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

                //options.UseSqlite(i_Config.GetConnectionString("DefaultConnection"));We chabge the database
                //options.UseNpgsql(i_Config.GetConnectionString("DefaultConnection"));

                var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

                string connStr;

                // Depending on if in development or production, use either Heroku-provided
                // connection string, or development connection string from env var.
                 if (environment == "Development")
                {
                
                    // Use connection string from file.
                    connStr = i_Config.GetConnectionString("DefaultConnection");

                }
                else
                {

                    // Use connection string provided at runtime by Heroku.
                    var connUrl = Environment.GetEnvironmentVariable("DATABASE_URL");

                         // Parse connection URL to connection string for Npgsql
                         connUrl = connUrl.Replace("postgres://", string.Empty);
                        var pgUserPass = connUrl.Split("@")[0];
                        var pgHostPortDb = connUrl.Split("@")[1];
                        var pgHostPort = pgHostPortDb.Split("/")[0];
                        var pgDb = pgHostPortDb.Split("/")[1];
                        var pgUser = pgUserPass.Split(":")[0];
                        var pgPass = pgUserPass.Split(":")[1];
                        var pgHost = pgHostPort.Split(":")[0];
                        var pgPort = pgHostPort.Split(":")[1];

                    connStr = $"Server={pgHost};Port={pgPort};User Id={pgUser};Password={pgPass};Database={pgDb};SSL Mode=Require;TrustServerCertificate=True";
   
           
                }

                //options.UseNpgsql(i_Config.GetConnectionString("DefaultConnection")); 
                
                // Whether the connection string came from the local development configuration file
                // or from the environment variable from Heroku, use it to set up your DbContext.
               options.UseNpgsql(connStr);
               
            });

        return i_Services;

        }

    }

}