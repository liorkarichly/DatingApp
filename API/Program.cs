using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            //Before the app started
            var host = CreateHostBuilder(args).Build();

            //I want to pass the service of my seed
            using var scope = host.Services.CreateScope();

            var service = scope.ServiceProvider;

            try
            {

                    //Require the service from data context and catch in problem
                var context = service.GetRequiredService<DataContext>();
                var userManger = service.GetRequiredService<UserManager<AppUser>>();
                var roleManager = service.GetRequiredService<RoleManager<AppRole>>();
                await context.Database.MigrateAsync();
                await Seed.SeedUsers(userManger, roleManager);

            }
            catch(Exception ex)
            {

                var logger = service.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occured during migration");

            }

            await host.RunAsync();
            
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
